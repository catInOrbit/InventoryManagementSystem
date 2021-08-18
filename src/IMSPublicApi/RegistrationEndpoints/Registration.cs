using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class Registration : BaseAsyncEndpoint.WithRequest<RegistrationRequest>.WithResponse<RegistrationResponse>
    {
        private readonly ITokenClaimsService _tokenClaimsService;
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
            OperationId = "auth.registration",
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
                            UserName = request.Email,
                            Address =  request.Address,
                            IsActive =  true,
                            DateOfBirth = request.DateOfBirth,
                            DateOfBirthNormalizedString = string.Format("{0}-{1}-{2}", request.DateOfBirth.Month, request.DateOfBirth.Day, request.DateOfBirth.Year),
                            ProfileImageLink = request.ProfileImageLink
                        };
                        
                        var checkDuplicateUser = await _userRoleModificationService.UserManager.FindByEmailAsync(request.Email);
                        if (checkDuplicateUser != null)
                        {
                            response.Result = false;
                            response.Verbose = "Error saving to DB, email already exists";
                            return NotFound(response);
                        }

                        var resultCreate = await _userRoleModificationService.UserManager.CreateAsync(newIMSUser, request.Password);
                        if (!resultCreate.Succeeded)
                        {
                            foreach (var resultCreateError in resultCreate.Errors)
                            {
                                response.Verbose += resultCreateError.Description;
                            }

                            response.Result = false;
                            return NotFound(response);
                        }
                        var result = await _userRoleModificationService.RoleCreatingHelper(newIMSUser.Id, role.Name);
                        
                        response.Result = result.Succeeded;
                        if (result.Succeeded)
                        {
                            response.Token = await _tokenClaimsService.GetTokenAsync(request.Email);
                            response.Verbose = "Authorized";
                            
                            //Save user notification info such as channel

                            response.UserAndRole = new UserAndRole
                            {
                                ImsUser = newIMSUser,
                                UserRole = role.Name,
                                RoleID = role.Id
                            };
                            return Ok(response);
                        }
                        else
                        {
                            response.Result = false;
                            response.Verbose = "Role does not exist in DB";
                            return NotFound(response);

                        }
                    }

                    else
                    {
                        response.Result = false;
                        response.Verbose = "Error saving to DB";
                        return NotFound(response);

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
            return NotFound(response);
        }


    }
}