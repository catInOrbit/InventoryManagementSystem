using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    [Authorize]
    public class RolePermissionUpdate : BaseAsyncEndpoint.WithRequest<RolePermissionRequest>.WithResponse<RolePermissionResponse>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly UserRoleModificationService _userRoleModificationService;
        private readonly IAuthorizationService _authorizationService;

        public UserInfo UserInfo { get; set; } = new UserInfo();

        public RolePermissionUpdate(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _userRoleModificationService = new UserRoleModificationService(_roleManager, _userManager);
        }

        [HttpPost("api/roleedit")]
        [SwaggerOperation(
            Summary = "Edit a role with permission (claim)",
            Description = "Edit a role with permission (claim)",
            OperationId = "manager.roleedit",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<RolePermissionResponse>> HandleAsync(RolePermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new RolePermissionResponse();
            var user = await _userRoleModificationService.UserManager.GetUserAsync(HttpContext.User);

            UserInfo.OwnerID = user.Id.ToString();
            // requires using ContactManager.Authorization;
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "RolePermissionUpdate",
                UserOperations.Check);

            if (isAuthorized.Succeeded)
            {
                foreach (var permissionClaimValue in request.PermissionClaimValues)
                {
                    foreach (var claimValue in permissionClaimValue.Value)
                    {
                        var result =  
                            await _userRoleModificationService.ClaimCreatingHelper(request.Role, new Claim(permissionClaimValue.Key, claimValue));

                        if (!result.Succeeded)
                        {
                            response.Result = false;
                            response.Verbose = "Error editing role, please try again";
                        }
                    }
                }
                
                response.Result = true;
                response.Verbose = "Success";
                response.RoleChanged = request.Role;
                return Ok(response);
            } 

            return Unauthorized();
        }
    }
}