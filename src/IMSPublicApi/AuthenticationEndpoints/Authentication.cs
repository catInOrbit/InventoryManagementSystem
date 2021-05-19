using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class Authentication : BaseAsyncEndpoint
        .WithRequest<AuthenticateRequest>
        .WithResponse<AuthenticateResponse>
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ITokenClaimsService _tokenClaimsService;

        public Authentication(SignInManager<IdentityUser> signInManager,
            ITokenClaimsService tokenClaimsService, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _tokenClaimsService = tokenClaimsService;
            _userManager = userManager;
        }

        [HttpPost("api/authentication")]
        [SwaggerOperation(
            Summary = "Authenticates a user",
            Description = "Authenticates a user",
            OperationId = "auth.authenticate",
            Tags = new[] { "IMSAuthenticationEndpoints" })
        ]
        public override async Task<ActionResult<AuthenticateResponse>> HandleAsync(AuthenticateRequest request, CancellationToken cancellationToken)
        {
            var response = new AuthenticateResponse(request.CorrelationId());

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            //var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
            var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);
            var user = await _userManager.FindByNameAsync(request.Username);
            var roles = await _userManager.GetRolesAsync(user);

            response.Result = result.Succeeded;
            response.IsLockedOut = result.IsLockedOut;
            response.IsNotAllowed = result.IsNotAllowed;
            response.RequiresTwoFactor = result.RequiresTwoFactor;
            response.Username = request.Username;
            response.UserRole = roles[0];

            if (result.Succeeded)
            {
                response.Token = await _tokenClaimsService.GetTokenAsync(request.Username);
            }

            return response;
        }
        
      
    }
}
