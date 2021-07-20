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
        private IUserSession _userAuthentication;

        public UpdateEndpoint(UserManager<ApplicationUser> userManager, IUserSession userAuthentication)
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
                if(request.ProfileImageLink != null ) userInfoGet.ProfileImageLink = request.ProfileImageLink;

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
                response.ApplicationUser = userInfoGet;
                return Ok(response);
            }
        }
    }
    
    public class UpdateImageEndpoint : BaseAsyncEndpoint.WithRequest<UpdateImageRequest>.WithResponse<UpdateResponse>
    {
        private UserManager<ApplicationUser> _userManager;
        private IUserSession _userAuthentication;

        public UpdateImageEndpoint(UserManager<ApplicationUser> userManager, IUserSession userAuthentication)
        {
            _userManager = userManager;
            _userAuthentication = userAuthentication;
        }

        [HttpPut("api/accountedit/image")]
        [SwaggerOperation(
            Summary = "Edit profile image information of account",
            Description = "Edit profile image information of account",
            OperationId = "users.editimage",
            Tags = new[] { "UserAccountEndpoints" })
        ]
        public override async Task<ActionResult<UpdateResponse>> HandleAsync(UpdateImageRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new UpdateResponse();

            var userSystemGet = await _userAuthentication.GetCurrentSessionUser();
            if(userSystemGet == null)
                return Unauthorized(response);  

            var userInfoGet = await _userManager.FindByIdAsync(userSystemGet.Id);

            if (userInfoGet == null)
                return Unauthorized(response);
            
            if(request.ProfileImageLink != null ) userInfoGet.ProfileImageLink = request.ProfileImageLink;

            await _userManager.UpdateAsync(userInfoGet);

            response.Result = true;
            response.Verbose = "Done updating profile image";
            response.ApplicationUser = userInfoGet;
            return Ok(response);
        }
    }
}