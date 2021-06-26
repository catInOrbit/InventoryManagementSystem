using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Cors;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
     
    public class RoleDelete : BaseAsyncEndpoint.WithRequest<RoleDeleteRequest>.WithResponse<RoleDeleteResponse>
    {
        private readonly UserRoleModificationService _userRoleModificationService;
        public readonly RoleManager<IdentityRole> _roleManager;
        private IUserSession _userAuthentication;

        private readonly IAuthorizationService _authorizationService;

        public RoleDelete(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, SignInManager<ApplicationUser> signInManager, IUserSession userAuthentication)
        {
            _roleManager = roleManager;
            _userRoleModificationService = new UserRoleModificationService(_roleManager);
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
        }
        
        [HttpPost("api/rolerm")]
        [SwaggerOperation(
           Summary = "Delete a role with all its permission (claim)",
           Description = "Delete a role with all its permission (claim)",
           OperationId = "manager.roledelete",
           Tags = new[] { "ManagerEndpoints" })
       ]
        public override async Task<ActionResult<RoleDeleteResponse>> HandleAsync(RoleDeleteRequest request, CancellationToken cancellationToken = default)
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.ROLEPERMISSION, UserOperations.Delete))
                return Unauthorized();

            
            var response = new RoleDeleteResponse();
            
            var userGet = await _userAuthentication.GetCurrentSessionUser();
            if(userGet == null)
                return Unauthorized();

            var isAuthorized = await _authorizationService.AuthorizeAsync(
              HttpContext.User, "RolePermissionUpdate",
              UserOperations.Update);

            IdentityResult result;
            if (isAuthorized.Succeeded)
            {
                result = await _userRoleModificationService.RoleDeletingHelper(request.Role);
                response.Result = true;
                response.Verbose = "Success";
                response.Role = request.Role;
                return Ok(response);
            }

            response.Result = false;
            response.Verbose = "Unauthorized";
            return Unauthorized(response);
        }
    }
}
