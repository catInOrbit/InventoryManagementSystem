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
        private IUserAuthentication _userAuthentication;

        private UserRoleModificationService _userRoleModificationService;
        public ApplicationUser UserInfo { get; set; } = new ApplicationUser();
        public Registration(UserManager<ApplicationUser> userManager, ITokenClaimsService tokenClaimsService,
            RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService , IUserAuthentication userAuthentication)
        {
            _tokenClaimsService = tokenClaimsService;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
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
                UserInfo.OwnerID = user.Id.ToString();
                // requires using ContactManager.Authorization;
                var isAuthorized = await _authorizationService.AuthorizeAsync(
                    HttpContext.User, "Registration",
                    UserOperations.Create);

                if (isAuthorized.Succeeded)
                {
                    var newUserID = await _userRoleModificationService.UserCreatingHelper(request.Password, request.Username, request.Email);
                    if (newUserID != null)
                    {
                        if (await _userRoleModificationService.CheckRoleNameExistsHelper(request.RoleName))
                        {
                            var result = await _userRoleModificationService.RoleCreatingHelper(newUserID, request.RoleName);
                    
                            var newIMSUser = new ApplicationUser
                            {
                                Id = newUserID,
                                Fullname =  request.FullName,
                                PhoneNumber =  request.PhoneNumber,
                                Email = request.Email,
                                UserName = request.FullName.Trim(),
                                Address =  request.Address,
                                IsActive =  true,
                                DateOfBirth = request.DateOfBirth,
                                DateOfBirthNormalizedString = string.Format("{0}-{1}-{2}", request.DateOfBirth.Month, request.DateOfBirth.Day, request.DateOfBirth.Year)
                            };
                    
                            await _userRoleModificationService.UserManager.CreateAsync(newIMSUser, request.Password);
                    
                            response.Result = result.Succeeded;
                            response.Username = request.FullName;
                            if (result.Succeeded)
                            {
                                response.Token = await _tokenClaimsService.GetTokenAsync(request.Email);
                                response.Verbose = "Authorized";
                            }
                        }

                        else
                        {
                            response.Result = false;
                            response.Verbose = "Role does not exist in DB";
                        }
                    }
                    else
                        response.Verbose = "Duplicated Email or Incorrect Request";
                }
                else
                    response.Verbose = "Not Authorized as Privileged User";
            }
            else
            {

                return Unauthorized();
            }

            
            return Ok(response);
        }


    }
}