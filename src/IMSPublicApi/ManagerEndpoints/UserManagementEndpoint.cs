using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class UserManagementEndpoint : BaseAsyncEndpoint.WithRequest<UserManagementRequest>.WithResponse<UserManagementResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private UserRoleModificationService _userRoleModificationService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementEndpoint(IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _authorizationService = authorizationService;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRoleModificationService = new UserRoleModificationService(roleManager, userManager);
        }
        
        [HttpPut("api/user/edit")]
        [SwaggerOperation(
            Summary = "Edit information of a user",
            Description = "Edit information a user",
            OperationId = "auth.useredit",
            Tags = new[] { "ManagerEndpoints" })
        ]

        public override async Task<ActionResult<UserManagementResponse>> HandleAsync(UserManagementRequest request, CancellationToken cancellationToken = new CancellationToken())
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
            user.UserName = user.Email;
            user.Address =  request.Address;
            user.IsActive =  true;
            user.DateOfBirth = request.DateOfBirth;
            user.DateOfBirthNormalizedString = string.Format("{0}-{1}-{2}", request.DateOfBirth.Month,
                request.DateOfBirth.Day, request.DateOfBirth.Year);
            user.ProfileImageLink = request.ProfileImageLink;

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

            var oldRoles = (await _userManager.GetRolesAsync(user));
            var newRole = await _userRoleModificationService.RoleManager.FindByIdAsync(request.RoleId);
            
            if(oldRoles.Count > 0)
                await _userRoleModificationService.UserManager.RemoveFromRoleAsync(user, oldRoles[0]);
            var result = await _userRoleModificationService.UserManager.AddToRoleAsync(user, newRole.Name);
            if (result.Succeeded)
            {
                var userRole = (await _userManager.GetRolesAsync(user))[0];
                var roleId = (await _roleManager.FindByNameAsync(userRole)).Id;
                
                response.Status = true;
                response.Verbose = "User updated";
                response.UserAndRole = new UserAndRole
                {
                  ImsUser  = user,
                  UserRole = userRole,
                  RoleID = roleId
                } ;
                return Ok(response);
            }
            
            else
            {
                return NotFound(result.Errors);
            }
        }
    }
}