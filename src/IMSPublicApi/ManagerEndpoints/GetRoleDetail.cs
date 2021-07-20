using System.Collections.Generic;
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
    public class GetRoleDetail : BaseAsyncEndpoint.WithRequest<GetSpecificRoleRequest>.WithResponse<GetSpecificRoleResponse>
    {
        private readonly UserRoleModificationService _userRoleService;
        private readonly IAuthorizationService _authorizationService;

        public GetRoleDetail(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _userRoleService = new UserRoleModificationService(roleManager);
        }

        [HttpGet("api/getrole/{RoleId}")]
        [SwaggerOperation(
            Summary = "Get detail of a specific role",
            Description = "Get detail of a specific role",
            OperationId = "manager.getRole",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<GetSpecificRoleResponse>> HandleAsync([FromRoute]GetSpecificRoleRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            
             
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.ROLEPERMISSION, UserOperations.Read))
                return Unauthorized();
            
            var response = new GetSpecificRoleResponse();
            response.Role = await _userRoleService.RoleManager.FindByIdAsync(request.RoleId);

            if (response.Role == null)
                return NotFound();
            
            var claims = await _userRoleService.RoleManager.GetClaimsAsync(response.Role);
            foreach (var claim in claims)
            {
                if (!response.PagePermissions.ContainsKey(claim.Type))
                {
                    response.PagePermissions.Add(claim.Type, new List<string>());
                }
                response.PagePermissions[claim.Type].Add(claim.Value);
             
            }
            // Dictionary<string, List<string>> pagePermissions = new Dictionary<string, List<string>>();
            // foreach (var claim in claims)
            // {
            //     if (!pagePermissions.ContainsKey(claim.Type))
            //     {
            //         pagePermissions.Add(claim.Type, new List<string>());
            //     }
            //     pagePermissions[claim.Type].Add(claim.Value);
            // }
            //
            // foreach (var keyValuePair in pagePermissions)
            // {
            //     response.PagePermissions.Add(new PagePermissions
            //     {
            //         Permissions = keyValuePair.Value,
            //         PageName = keyValuePair.Key
            //     });
            //
            // }
            return Ok(response);
        }
    }
}