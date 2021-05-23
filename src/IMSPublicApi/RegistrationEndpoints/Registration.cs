using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    [Authorize]
    public class Registration : BaseAsyncEndpoint.WithRequest<RegistrationRequest>.WithResponse<RegistrationResponse>
    {
        private readonly ITokenClaimsService _tokenClaimsService;
        // private readonly UserManager<ApplicationUser> _userManager;
        // private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<UserInfo> _userRepository;
        private UserRoleModificationService _userRoleModificationService;
        public UserInfo UserInfo { get; set; } = new UserInfo();
        public Registration(UserManager<ApplicationUser> userManager, ITokenClaimsService tokenClaimsService,
            RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, IAsyncRepository<UserInfo> userRepository)
        {
            _tokenClaimsService = tokenClaimsService;
            _authorizationService = authorizationService;
            _userRepository = userRepository;
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
            var user = await _userRoleModificationService.UserManager.GetUserAsync(HttpContext.User);
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
                        var result = await _userRoleModificationService.RoleCreatingHelper(newUserID, request.RoleName);
                    
                        var newIMSUser = new UserInfo
                        {
                            Id = newUserID,
                            Fullname =  request.FullName,
                            PhoneNumber =  request.PhoneNumber,
                            Email = user.Email,
                            Username = request.FullName.Trim(),
                            Address =  request.Address,
                            IsActive =  true,
                            DateOfBirth = request.DateOfBirth
                        };
                    
                        await _userRepository.AddAsync(newIMSUser, cancellationToken);
                    
                        response.Result = result.Succeeded;
                        response.Username = request.FullName;
                        if (result.Succeeded)
                        {
                            response.Token = await _tokenClaimsService.GetTokenAsync(request.Email);
                            response.Verbose = "Authorized";
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