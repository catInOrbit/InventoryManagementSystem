using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RolePermissionUpdate : BaseAsyncEndpoint.WithRequest<RolePermissionRequest>.WithResponse<RolePermissionResponse>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private UserRoleModificationService _userRoleModificationService;
        
       
        public RolePermissionUpdate(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userRoleModificationService = new UserRoleModificationService(_roleManager);
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
            
            foreach (var permissionClaimValue in request.PermissionClaimValues)
            {
                var result =  
                    await _userRoleModificationService.ClaimCreatingHelper(request.Role, new Claim(permissionClaimValue.Key, permissionClaimValue.Value));

                if (!result.Succeeded)
                {
                    response.Result = false;
                    response.Verbose = "Error editing role, please try again";
                }
            }
            
            response.Result = true;
            response.Verbose = "Success";
            response.RoleChanged = request.Role;
            return response;
        }
    }
}