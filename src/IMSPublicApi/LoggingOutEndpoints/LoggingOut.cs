using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.LoggingOutEndpoints
{
   
    public class LoggingOut : BaseAsyncEndpoint.WithoutRequest.WithResponse<LoggingOutResponse>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserSession _userSession;

        public LoggingOut(SignInManager<ApplicationUser> signInManager, IUserSession userSession)
        {
            _signInManager = signInManager;
            _userSession = userSession;
        }

        [HttpPost("api/useroff")]
        [SwaggerOperation(
            Summary = "Logging off a user",
            Description = "Logging a user",
            OperationId = "loggingout",
            Tags = new[] { "LoggingOutEndpoints" })
        ]
        public override async Task<ActionResult<LoggingOutResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new LoggingOutResponse();
            await _userSession.RemoveUserAsync(await _userSession.GetCurrentSessionUser());
            await _signInManager.SignOutAsync();
            
            response.Result = true;
            response.Verbose = "Logged out user";

            return response;
        }
    }
}