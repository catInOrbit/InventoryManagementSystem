using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class Registration : BaseAsyncEndpoint.WithRequest<RegistrationRequest>.WithResponse<RegistrationResponse>
    {
        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthorizationService _authorizationService;
        private IUserSession _userAuthentication;

        private UserRoleModificationService _userRoleModificationService;
        private IAsyncRepository<Notification> _notificationAsyncRepository;

        private readonly INotificationService _notificationService;

        public ApplicationUser UserInfo { get; set; } = new ApplicationUser();
        public Registration(UserManager<ApplicationUser> userManager, ITokenClaimsService tokenClaimsService,
            RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService , IUserSession userAuthentication, IAsyncRepository<Notification> notificationAsyncRepository, INotificationService notificationService)
        {
            _tokenClaimsService = tokenClaimsService;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationAsyncRepository = notificationAsyncRepository;
            _notificationService = notificationService;
            _userRoleModificationService = new UserRoleModificationService(roleManager, userManager);
        }
        
        [HttpPost("api/registration")]
        [SwaggerOperation(
            Summary = "Registration of a user",
            Description = "Registration a user",
            OperationId = "auth.registertest",
            Tags = new[] { "IMSRegisterEndpoints" })
        ]
        public override async Task<ActionResult<RegistrationResponse>> HandleAsync(RegistrationRequest request, CancellationToken cancellationToken)
        {
            var response = new RegistrationResponse(request.CorrelationId());
            var userGet = await _userAuthentication.GetCurrentSessionUser();
            if(userGet == null)
                return Unauthorized();
            
            
            var user = userGet;
            if (user != null)
            {
                try
                {
                    UserInfo.OwnerID = user.Id.ToString();
                // requires using ContactManager.Authorization;
                var isAuthorized = await _authorizationService.AuthorizeAsync(
                    HttpContext.User, "Registration",
                    UserOperations.Create);
        
                var role = await _userRoleModificationService.RoleManager.FindByIdAsync(request.RoleId);
                if (role == null)
                {
                    response.Result = false;
                    response.Verbose = "Role does not exist in DB";
                    return Ok(response);
                }
                if (isAuthorized.Succeeded)
                {
                    if (await _userRoleModificationService.CheckRoleNameExistsHelper(role.Name))
                    {
                        var newIMSUser = new ApplicationUser
                        {
                            Fullname =  request.FullName,
                            PhoneNumber =  request.PhoneNumber,
                            Email = request.Email,
                            UserName = request.FullName.Replace(" ",""),
                            Address =  request.Address,
                            IsActive =  true,
                            DateOfBirth = request.DateOfBirth,
                            DateOfBirthNormalizedString = string.Format("{0}-{1}-{2}", request.DateOfBirth.Month, request.DateOfBirth.Day, request.DateOfBirth.Year)
                        };

                        var resultCreate = await _userRoleModificationService.UserManager.CreateAsync(newIMSUser, request.Password);
                        if (!resultCreate.Succeeded)
                        {
                            foreach (var resultCreateError in resultCreate.Errors)
                            {
                                response.Verbose += resultCreateError.Description;
                            }

                            response.Result = false;
                            return Ok(response);
                        }
                        var result = await _userRoleModificationService.RoleCreatingHelper(newIMSUser.Id, role.Name);
                        
                        response.Result = result.Succeeded;
                        response.Username = request.FullName;
                        if (result.Succeeded)
                        {
                            response.Token = await _tokenClaimsService.GetTokenAsync(request.Email);
                            response.Verbose = "Authorized";
                            
                            //Save user notification info such as channel
                            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
                            var messageNotification =
                                _notificationService.CreateMessage(currentUser.Fullname, "Create","New User", user.Id);
            
                            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                                currentUser.Id, messageNotification);
                            return Ok(response);
                        }
                        else
                        {
                            response.Result = false;
                            response.Verbose = "Role does not exist in DB";
                            return Ok(response);

                        }
                    }

                    else
                    {
                        response.Result = false;
                        response.Verbose = "Error saving to DB";
                        return Ok(response);

                    }
                }
                else
                    response.Verbose = "Not Authorized as Privileged User";

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {

                return Unauthorized();
            }

            return NotFound();
        }


    }
}