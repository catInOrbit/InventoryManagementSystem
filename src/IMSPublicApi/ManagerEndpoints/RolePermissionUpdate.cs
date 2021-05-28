using System.Linq;
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
        private IUserAuthentication _userAuthentication;

        public UserInfo UserInfo { get; set; } = new UserInfo();

        public RolePermissionUpdate(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IUserAuthentication userAuthentication)
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
            var userGet = _userAuthentication.GetCurrentSessionUser();
            if(userGet == null)
                return Unauthorized();
            
            var response = new RolePermissionResponse();
            var allRoles = _userRoleModificationService.RoleManager.Roles.ToList();
            UserInfo.OwnerID = userGet.Id.ToString();
            // requires using ContactManager.Authorization;
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "RolePermissionUpdate",
                UserOperations.Update);

            if (isAuthorized.Succeeded)
            {
                if(allRoles.Contains( new IdentityRole(request.Role.ToString())))
                    await _userRoleModificationService.RemoveAllClaimHelper(request.Role);

                // page -- list<action>
                foreach (var pageClaimKeyValuePair in request.PageClaimDictionary)
                {
                    foreach (var pageClaim in pageClaimKeyValuePair.Value)
                    {
                        var result =
                           await _userRoleModificationService.ClaimCreatingHelper(request.Role, new Claim(pageClaimKeyValuePair.Key, pageClaim));

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