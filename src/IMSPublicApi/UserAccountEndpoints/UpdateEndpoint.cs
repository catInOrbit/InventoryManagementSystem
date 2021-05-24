using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity.Models;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UserAccountEndpoints
{
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class UpdateEndpoint : BaseAsyncEndpoint.WithRequest<UpdateRequest>.WithResponse<UpdateResponse>
    {
        private IAsyncRepository<UserInfo> _asyncRepository;
        private UserManager<ApplicationUser> _userManager;

        public UpdateEndpoint(IAsyncRepository<UserInfo> asyncRepository, UserManager<ApplicationUser> userManager)
        {
            _asyncRepository = asyncRepository;
            _userManager = userManager;
        }

        [HttpPut("api/accountedit")]
        [SwaggerOperation(
            Summary = "Edit information of account",
            Description = "Edit information of account",
            OperationId = "users.edit",
            Tags = new[] { "UserAccountEndpoints" })
        ]
        public override async Task<ActionResult<UpdateResponse>> HandleAsync(UpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new UpdateResponse();
            var userID = _userManager.GetUserId(HttpContext.User);
            
            var userGet = await _asyncRepository.GetByIdAsync(userID, cancellationToken);

            userGet.Address = request.Address;
            // userGet.Email = request.Email;
            userGet.Fullname = request.Fullname;
            userGet.Username = request.Username;
            userGet.IsActive = request.IsActive;
            userGet.PhoneNumber = request.PhoneNumber;
            userGet.DateOfBirth = request.DateOfBirth;
            
            await _asyncRepository.UpdateAsync(userGet, cancellationToken);

            response.Result = true;
            response.Verbose = "Done updating";

            return Ok(response);
        }
    }
}