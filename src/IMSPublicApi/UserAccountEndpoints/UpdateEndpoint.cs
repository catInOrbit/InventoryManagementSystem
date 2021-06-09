using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UserAccountEndpoints
{
    public class UpdateEndpoint : BaseAsyncEndpoint.WithRequest<UpdateRequest>.WithResponse<UpdateResponse>
    {
        private UserManager<ApplicationUser> _userManager;
        private IUserAuthentication _userAuthentication;

        public UpdateEndpoint(UserManager<ApplicationUser> userManager, IUserAuthentication userAuthentication)
        {
            _userManager = userManager;
            _userAuthentication = userAuthentication;
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

            var userSystemGet = await _userAuthentication.GetCurrentSessionUser();
            if(userSystemGet == null)
                return Unauthorized(response);  

            else
            {
                var userInfoGet = await _userManager.FindByIdAsync(userSystemGet.Id);
                if (userInfoGet == null)
                    return Unauthorized(response);
                if(request.Address != userInfoGet.Address ) userInfoGet.Address = request.Address;
                // userGet.Email = request.Email;
                if(request.Fullname != userInfoGet.Fullname ) userInfoGet.Fullname = request.Fullname;
                if(request.PhoneNumber != userInfoGet.PhoneNumber ) userInfoGet.PhoneNumber = request.PhoneNumber;
                if (request.DateOfBirth != userInfoGet.DateOfBirth)
                {
                    userInfoGet.DateOfBirth = request.DateOfBirth.Date;
                    userInfoGet.DateOfBirthNormalizedString = string.Format("{0}-{1}-{2}", request.DateOfBirth.Year, request.DateOfBirth.Month, request.DateOfBirth.Day);
                }

                if (request.NewPassword != null)
                {
                    var result = await _userManager.ChangePasswordAsync(userSystemGet, request.OldPassword, request.NewPassword);
                    if(!result.Succeeded)
                    {
                        response.Result = false;
                        response.Verbose = "Wrong password";
                        return Ok(response);
                    }
                }

                await _userManager.UpdateAsync(userInfoGet);

                response.Result = true;
                response.Verbose = "Done updating";

                return Ok(response);
            }
        }
    }
}