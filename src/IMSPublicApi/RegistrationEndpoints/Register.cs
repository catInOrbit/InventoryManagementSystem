using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class Register : BaseAsyncEndpoint.WithRequest<RegisterRequest>.WithResponse<RegisterResponse>
    {
        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceProvider _serviceProvider;

        public Register(UserManager<IdentityUser> userManager, ITokenClaimsService tokenClaimsService,
            RoleManager<IdentityRole> roleManager, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _tokenClaimsService = tokenClaimsService;
            _roleManager = roleManager;
            _serviceProvider = serviceProvider;
        }
        
        [HttpPost("api/registertest")]
        [SwaggerOperation(
            Summary = "Test Registration of a user",
            Description = "Test Registration a user",
            OperationId = "auth.registertest",
            Tags = new[] { "IMSRegisterEndpoints" })
        ]
        public override async Task<ActionResult<RegisterResponse>> HandleAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = new RegisterResponse(request.CorrelationId());

            var newUserID = await UserCreatimgHelper(_serviceProvider, request.Password, request.Username, request.Email);
            var result = await RoleCreatingHelper(_serviceProvider, newUserID, request.RoleName);
            
            // var user = new IdentityUser { UserName = "test", Email = "Test@gmail.com" };
            // var result = await _userManager.CreateAsync(user, "testPass#1234");

            response.Result = result.Succeeded;
            response.Username = request.Username;

            if (result.Succeeded)
            {
                // response.Token = await _tokenClaimsService.GetTokenAsync(request.Username);
                response.Token = "Test";
            }

            return response;
        }

        private async Task<string> UserCreatimgHelper(IServiceProvider serviceProvider, string password, string username, string email)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user, password);
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
            
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

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