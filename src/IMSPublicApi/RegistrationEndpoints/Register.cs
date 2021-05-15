using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class Register : BaseAsyncEndpoint.WithRequest<RegisterRequest>.WithResponse<RegisterResponse>
    {
        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly UserManager<IdentityUser> _userManager;

        public Register(UserManager<IdentityUser> userManager, ITokenClaimsService tokenClaimsService)
        {
            _userManager = userManager;
            _tokenClaimsService = tokenClaimsService;
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

            var user = new IdentityUser { UserName = "test", Email = "Test@gmail.com" };
            var result = await _userManager.CreateAsync(user, "testPass#1234");

            response.Result = result.Succeeded;
            response.Username = request.Username;

            if (result.Succeeded)
            {
                // response.Token = await _tokenClaimsService.GetTokenAsync(request.Username);
                response.Token = "Test";
            }

            return response;
        }
    }
}