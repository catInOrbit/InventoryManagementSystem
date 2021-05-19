using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ResetPasswordEndpoints
{
    public class ResetPasswordSubmit : BaseAsyncEndpoint
        .WithRequest<ResetPasswordSubmitRequest>.WithResponse<ResetPasswordSubmitResponse>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetPasswordSubmit(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        

        [HttpPost("api/resetpasssubmit")]
        [SwaggerOperation(
            Summary = "Submit password reset POST action",
            Description = "Submit password reset POST action",
            OperationId = "resetpasssubmit",
            Tags = new[] { "ResetPasswordEndpoints" })
        ]
        public override async Task<ActionResult<ResetPasswordSubmitResponse>> HandleAsync(ResetPasswordSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var response = new ResetPasswordSubmitResponse(request.CorrelationId());
            response.Result = false;
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
                if (result.Succeeded)
                {
                    response.Result = result.Succeeded;
                    response.Username = user.UserName;
                }
            }
            return response;
        }
    }
}