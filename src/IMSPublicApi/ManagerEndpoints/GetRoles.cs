using System.Linq;
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
    public class GetRoles : BaseAsyncEndpoint.WithRequest<GetAllRoleRequest>.WithResponse<GetAllRoleResponse>
    {        
        private IUserSession _userAuthentication;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthorizationService _authorizationService;

        public GetRoles(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService, IUserSession userAuthentication)
        {
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
        }

        [HttpGet("api/getroles")]
        [SwaggerOperation(
            Summary = "Get all roles of system",
            Description = "Get all roles of system",
            OperationId = "auth.getroles",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<GetAllRoleResponse>> HandleAsync([FromQuery]GetAllRoleRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var userGet = await _userAuthentication.GetCurrentSessionUser();
            if(userGet == null)
                return Unauthorized();
            
            //
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.ROLEPERMISSION, UserOperations.Read))
                return Unauthorized();

            PagingOption<IdentityRole> pagingOption =
                new PagingOption<IdentityRole>(request.CurrentPage, request.SizePerPage);
            var response = new GetAllRoleResponse();
            var roleList =  _roleManager.Roles.ToList();

            // List<string> roles = new List<string>();
            // foreach (var role in roleList)
            // {
            //     roles.Add(role.Name);
            // }

            pagingOption.ResultList = roleList;
            response.Paging = pagingOption;
            response.Paging.ExecuteResourcePaging();
            return Ok(response);
        }
    }
}