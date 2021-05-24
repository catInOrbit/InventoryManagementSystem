using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class RoleUserAssign : BaseAsyncEndpoint.WithRequest<RoleUserAssignRequest>.WithResponse<RoleUserAssignResponse>
    {
        private UserRoleModificationService _userRoleModificationService;
        private readonly IAuthorizationService _authorizationService;

        public RoleUserAssign(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _userRoleModificationService = new UserRoleModificationService(roleManager, userManager);
        }
        
        
        [HttpPost("api/roleuserassign")]
        [SwaggerOperation(
            Summary = "Assign user to a role",
            Description = "Assign user to a role",
            OperationId = "role.roleuserassign",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<RoleUserAssignResponse>> HandleAsync(RoleUserAssignRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new RoleUserAssignResponse();
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "RoleUserAssign",
                UserOperations.Update);
            IdentityResult result = new IdentityResult();
            if(isAuthorized.Succeeded)
                 result = await _userRoleModificationService.RoleUpdatingHelper(request.UserID, request.NewRole);

            if (result.Succeeded)
                response.Result = true;
            
            return Ok(response);
        }
    }
}