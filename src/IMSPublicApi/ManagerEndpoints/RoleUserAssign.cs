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
     
    public class RoleUserAssign : BaseAsyncEndpoint.WithRequest<RoleUserAssignRequest>.WithResponse<RoleUserAssignResponse>
    {
        private UserRoleModificationService _userRoleModificationService;
        private readonly IAuthorizationService _authorizationService;
        private IUserSession _userAuthentication;

        public RoleUserAssign(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, IUserSession userAuthentication)
        {
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
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
            
            var userGet = await _userAuthentication.GetCurrentSessionUser();
            if(userGet == null)
                return Unauthorized();
            
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User,  PageConstant.ROLEPERMISSION,
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