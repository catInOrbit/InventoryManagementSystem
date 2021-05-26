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
            
            var userInfoGet = await _asyncRepository.GetByIdAsync(userID, cancellationToken);
            var userSystem = await _userManager.GetUserAsync(HttpContext.User);

            if(request.Address != userInfoGet.Address ) userInfoGet.Address = request.Address;
            // userGet.Email = request.Email;
            if(request.Fullname != userInfoGet.Fullname ) userInfoGet.Fullname = request.Fullname;
            if(request.PhoneNumber != userInfoGet.PhoneNumber ) userInfoGet.PhoneNumber = request.PhoneNumber;
            if(request.DateOfBirth != userInfoGet.DateOfBirth ) userInfoGet.DateOfBirth = request.DateOfBirth;

            if (request.NewPassword != null)
            {
                var result = await _userManager.ChangePasswordAsync(userSystem, request.OldPassword, request.NewPassword);
                if(!result.Succeeded)
                {
                    response.Result = false;
                    response.Verbose = "Wrong password";
                    return Ok(response);
                }
            }

            await _asyncRepository.UpdateAsync(userInfoGet, cancellationToken);

            response.Result = true;
            response.Verbose = "Done updating";

            return Ok(response);
        }
    }
}