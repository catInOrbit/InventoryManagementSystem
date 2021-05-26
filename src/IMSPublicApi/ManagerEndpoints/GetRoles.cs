using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    [Authorize]
    public class GetRoles : BaseAsyncEndpoint.WithoutRequest.WithResponse<GetRoleResponse>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthorizationService _authorizationService;

        public GetRoles(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService)
        {
            _roleManager = roleManager;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/getroles")]
        [SwaggerOperation(
            Summary = "Get all roles of system",
            Description = "Get all roles of system",
            OperationId = "auth.registertest",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<GetRoleResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "Registration",
                UserOperations.Read);


            if (isAuthorized.Succeeded)
            {
                var response = new GetRoleResponse();
                var roleList =  _roleManager.Roles.ToList();

                List<string> roles = new List<string>();
                foreach (var role in roleList)
                {
                    roles.Add(role.Name);
                }

                response.Roles = roles;
                return Ok(response);
            }

            return Unauthorized();

        }
    }
}