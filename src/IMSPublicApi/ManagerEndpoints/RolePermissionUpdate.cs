using System.Linq;
using System.Security.Claims;
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

        [HttpPut("api/role/edit")]
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
            UserInfo.OwnerID = userGet.Id.ToString();

            var oldRole = await _userRoleModificationService.RoleManager.FindByIdAsync(request.RoleId);
            oldRole.Name = request.RoleName;
            await _userRoleModificationService.RoleManager.UpdateAsync(oldRole);
            
            if(oldRole != null)
                await _userRoleModificationService.RemoveAllClaimHelper(oldRole);

            // page -- list<action>
            foreach (var pageClaimKeyValuePair in request.PageClaimDictionary)
            {
                foreach (var pageClaim in pageClaimKeyValuePair.Value)
                { 
                    var result =
                       await _userRoleModificationService.ClaimCreatingHelper(oldRole.Name, new Claim(pageClaimKeyValuePair.Key, pageClaim));
            
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
            response.RoleChanged = oldRole;
            return Ok(response);
        }
    }
    
     public class RolePermissionCreate : BaseAsyncEndpoint.WithRequest<RolePermissionCreateRequest>.WithResponse<RolePermissionResponse>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly UserRoleModificationService _userRoleModificationService;
        private readonly IAuthorizationService _authorizationService;
        private IUserSession _userAuthentication;

        public ApplicationUser UserInfo { get; set; } = new ApplicationUser();

        public RolePermissionCreate(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IUserSession userAuthentication)
        {
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _userAuthentication = userAuthentication;
            _userRoleModificationService = new UserRoleModificationService(_roleManager, _userManager);
        }

        [HttpPost("api/role/create")]
        [SwaggerOperation(
            Summary = "Edit a role with permission (claim), creating new one if there's none",
            Description = "Edit a role with permission (claim)",
            OperationId = "manager.rolecreate",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<RolePermissionResponse>> HandleAsync(RolePermissionCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        { 
            var userGet = await _userAuthentication.GetCurrentSessionUser();
            if(userGet == null)
                return Unauthorized();
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.ROLEPERMISSION, UserOperations.Update))
                return Unauthorized();
            
            var response = new RolePermissionResponse();
            var allRoles = _userRoleModificationService.RoleManager.Roles.ToList();
            UserInfo.OwnerID = userGet.Id.ToString();

            var newRole = new IdentityRole(request.RoleName);

            // page -- list<action>
            foreach (var pageClaimKeyValuePair in request.PageClaimDictionary)
            {
                foreach (var pageClaim in pageClaimKeyValuePair.Value)
                { 
                    var result =
                       await _userRoleModificationService.ClaimCreatingHelper(newRole.Name, new Claim(pageClaimKeyValuePair.Key, pageClaim));
            
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
            response.RoleChanged = newRole;
            return Ok(response);
        }
    }
}