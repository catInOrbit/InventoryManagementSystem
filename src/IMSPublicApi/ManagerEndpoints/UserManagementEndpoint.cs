using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class UserManagementEndpoint : BaseAsyncEndpoint.WithRequest<UserManagementRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private UserRoleModificationService _userRoleModificationService;

        public UserManagementEndpoint(IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _authorizationService = authorizationService;
            _userRoleModificationService = new UserRoleModificationService(roleManager, userManager);
        }
        
        [HttpPut("api/user/edit")]
        [SwaggerOperation(
            Summary = "Edit information of a user",
            Description = "Edit information a user",
            OperationId = "auth.registertest",
            Tags = new[] { "ManagerEndpoints" })
        ]

        public override async Task<ActionResult> HandleAsync(UserManagementRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.USERDETAIL, UserOperations.Update))
                return Unauthorized();

            var response = new UserManagementResponse();
            var user = await _userRoleModificationService.UserManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound();

            user.Fullname =  request.Fullname;
            user.PhoneNumber =  request.PhoneNumber;
            user.Email = request.Email;
            user.UserName = request.Fullname.Replace(" ","");
            user.Address =  request.Address;
            user.IsActive =  true;
            user.DateOfBirth = request.DateOfBirth;
            user.DateOfBirthNormalizedString = string.Format("{0}-{1}-{2}", request.DateOfBirth.Month,
                request.DateOfBirth.Day, request.DateOfBirth.Year);

            await _userRoleModificationService.UserManager.UpdateAsync(user);
            
            if (request.NewPassword != null)
            {
                var resultPassword = await _userRoleModificationService.UserManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                if(!resultPassword.Succeeded)
                {
                    response.Status = false;
                    response.Verbose = "Password's incorrect or not strong enough";
                    return NotFound(response);
                }
            }

            if (user.Email == "tmh1799@gmail.com")
            {
                response.Status = true;
                response.Verbose = "Info updated, But Role of user tmh1799@gmail.com: Manager can not be changed";
                return Ok(response);
            }
            
            var role = await _userRoleModificationService.RoleManager.FindByIdAsync(request.RoleId);
            await _userRoleModificationService.UserManager.RemoveFromRoleAsync(user, role.Name);
            var result = await _userRoleModificationService.RoleCreatingHelper(user.Id, role.Name);

            if (result.Succeeded)
            {
                response.Status = true;
                response.Verbose = "User updated";
                response.UserUpdated = user;
                return Ok(response);
            }
            
            else
            {
                return NotFound(result.Errors);
            }
        }
    }
}