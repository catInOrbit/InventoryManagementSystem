using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
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
     
    public class RolePermissionUpdate : BaseAsyncEndpoint.WithRequest<RolePermissionRequest>.WithResponse<RolePermissionResponse>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly UserRoleModificationService _userRoleModificationService;
        private readonly IAuthorizationService _authorizationService;
        private IUserSession _userAuthentication;

        public ApplicationUser UserInfo { get; set; } = new ApplicationUser();

        public RolePermissionUpdate(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IUserSession userAuthentication)
        {
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _userAuthentication = userAuthentication;
            _userRoleModificationService = new UserRoleModificationService(_roleManager, _userManager);
        }

        [HttpPut("api/roleedit")]
        [SwaggerOperation(
            Summary = "Edit a role with permission (claim), creating new one if there's none",
            Description = "Edit a role with permission (claim)",
            OperationId = "manager.roleedit",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<RolePermissionResponse>> HandleAsync(RolePermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
        { 
            var userGet = await _userAuthentication.GetCurrentSessionUser();
            if(userGet == null)
                return Unauthorized();
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.ROLEPERMISSION, UserOperations.Update))
                return Unauthorized();
            
            var response = new RolePermissionResponse();
            var allRoles = _userRoleModificationService.RoleManager.Roles.ToList();
            UserInfo.OwnerID = userGet.Id.ToString();
            

            if(allRoles.Contains( new IdentityRole(request.Role.ToString())))
                await _userRoleModificationService.RemoveAllClaimHelper(request.Role);

            // foreach (var requestPagePermission in request.PagePermissions)
            // {
            //     foreach (var permission in requestPagePermission.Permissions)
            //     {
            //         var result =
            //            await _userRoleModificationService.ClaimCreatingHelper(request.Role, request.RoleDescription, new Claim(requestPagePermission.PageName, permission));
            //         if (!result.Succeeded)
            //         {
            //             response.Result = false;
            //             response.Verbose = "Error editing role, please try again";
            //             return Ok(response);
            //         }
            //     }
            // }
            // page -- list<action>
            foreach (var pageClaimKeyValuePair in request.PageClaimDictionary)
            {
                foreach (var pageClaim in pageClaimKeyValuePair.Value)
                { 
                    var result =
                       await _userRoleModificationService.ClaimCreatingHelper(request.Role, request.RoleDescription, new Claim(pageClaimKeyValuePair.Key, pageClaim));
            
                    if (!result.Succeeded)
                    {
                        response.Result = false;
                        response.Verbose = "Error editing role, please try again";
                        return Ok(response);
                    }
                }
            }
            
            response.Result = true;
            response.Verbose = "Success";
            response.RoleChanged = request.Role;
            return Ok(response);

            return Unauthorized();
        }
    }
}