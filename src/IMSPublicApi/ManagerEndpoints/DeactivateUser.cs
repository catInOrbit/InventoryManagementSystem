using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class DeactivateUser : BaseAsyncEndpoint.WithRequest<DeactivateUserRequest>.WithResponse<UserAndRole>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IAuthorizationService _authorizationService;

        public DeactivateUser(UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _roleManager = roleManager;
        }

        [HttpPut("api/deactivate")]
        [SwaggerOperation(
            Summary = "Deactivate or activate a user",
            Description = "Deactivate or activate a user",
            OperationId = "manager.deactivate",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<UserAndRole>> HandleAsync(DeactivateUserRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.USERDETAIL, UserOperations.Update))
                return Unauthorized();

            var response = new UserAndRole();
            
            var userGet = await _userManager.FindByIdAsync(request.UserId);
            userGet.IsActive = request.IsDeactivated;
            response.ImsUser = userGet;
            response.UserRole = (await _userManager.GetRolesAsync(userGet))[0];
            response.RoleID = (await _roleManager.FindByNameAsync(response.UserRole)).Id;
            await _userManager.UpdateAsync(userGet);
            return Ok(response);
        }
    }
}