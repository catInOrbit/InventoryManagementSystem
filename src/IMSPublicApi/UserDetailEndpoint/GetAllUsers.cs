using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
    public class GetAllUsers : BaseAsyncEndpoint.WithRequest<GetAllUserRequest>.WithResponse<UsersResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IAuthorizationService _authorizationService;
        private IUserSession _userAuthentication;

        public ApplicationUser UserInfo { get; set; } = new ApplicationUser();

        public GetAllUsers(UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService, IUserSession userAuthentication, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _roleManager = roleManager;
        }

        [HttpGet("api/users")]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Gets all users",
            OperationId = "users.GetAll",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<UsersResponse>> HandleAsync([FromQuery]GetAllUserRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.USERDETAIL, UserOperations.Read))
                return Unauthorized();
            
            var response = new UsersResponse();
            
            var user = await _userAuthentication.GetCurrentSessionUser();
            if(user == null)
                return Unauthorized();
            
            UserInfo.OwnerID = user.Id.ToString();
            
            PagingOption<UserAndRole> pagingOption =
                new PagingOption<UserAndRole>(request.CurrentPage, request.SizePerPage);
            
            if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var users = await _userManager.Users.ToListAsync();
                foreach (var applicationUser in users)
                {
                    var userAndRole = new UserAndRole();
                    userAndRole.ImsUser = applicationUser;
                    var userRoles = await _userManager.GetRolesAsync(applicationUser);
                    if (userRoles.Count != 0)
                    {
                        userAndRole.UserRole =  (await _userManager.GetRolesAsync(applicationUser))[0];
                        var getRole = await _roleManager.FindByNameAsync(userAndRole.UserRole);
                        userAndRole.RoleID = getRole.Id;
                    }
                    
                    pagingOption.ResultList.Add(userAndRole);
                }

                pagingOption.ResultList = FilteringService.UserAndRolesIndexFiltering(pagingOption.ResultList.ToList(), request);
                response.Paging = pagingOption;
                response.Paging.ExecuteResourcePaging();
                return Ok(response);
            }

            return Unauthorized();

        }
    }
}
