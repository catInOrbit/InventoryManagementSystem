using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    [Authorize]
    public class Registration : BaseAsyncEndpoint.WithRequest<RegistrationRequest>.WithResponse<RegistrationResponse>
    {
        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<IMSUser> _userRepository;
        
        public IMSUser IMSUser { get; set; } = new IMSUser();
        public Registration(UserManager<ApplicationUser> userManager, ITokenClaimsService tokenClaimsService,
            RoleManager<IdentityRole> roleManager, IServiceProvider serviceProvider, IAuthorizationService authorizationService, IAsyncRepository<IMSUser> userRepository)
        {
            _userManager = userManager;
            _tokenClaimsService = tokenClaimsService;
            _roleManager = roleManager;
            _serviceProvider = serviceProvider;
            _authorizationService = authorizationService;
            _userRepository = userRepository;
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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                IMSUser.OwnerID = user.Id.ToString();
                // requires using ContactManager.Authorization;
                var isAuthorized = await _authorizationService.AuthorizeAsync(
                    HttpContext.User, IMSUser,
                    UserOperations.Create);

                if (isAuthorized.Succeeded)
                {
                    var newUserID = await UserCreatingHelper(_serviceProvider, request.Password, request.Username, request.Email);
                    if (newUserID != null)
                    {
                        var result = await RoleCreatingHelper(_serviceProvider, newUserID, request.RoleName);
                    
                        var newIMSUser = new IMSUser
                        {
                            Id = newUserID,
                            Fullname =  request.FullName,
                            PhoneNumber =  request.PhoneNumber,
                        
                            Address =  request.Address,
                            IsActive =  true,
                            DateOfBirth = request.DateOfBirth
                        };
                    
                        await _userRepository.AddAsync(newIMSUser, cancellationToken);
                    
                        response.Result = result.Succeeded;
                        response.Username = request.Username;

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
                    response.Verbose = "Not Authorized";
            }
            
            else
            {
                
                response.Verbose = "Not authorized";
            }

            
            return response;
        }

        private async Task<string> UserCreatingHelper(IServiceProvider serviceProvider, string password, string username, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };
                
                await _userManager.CreateAsync(user, password);
            }

            else
            {
                user.Id = null;
            }
            //TODO: Throw Error saying authentication fail
            return user.Id;
        }

        private async Task<IdentityResult> RoleCreatingHelper(IServiceProvider serviceProvider,
            string uid, string role)
        {
            IdentityResult result = null;
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (_roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                result = await _roleManager.CreateAsync(new IdentityRole(role));
            }
            
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if(user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }
            
            result = await userManager.AddToRoleAsync(user, role);

            return result;
        }
    }
}