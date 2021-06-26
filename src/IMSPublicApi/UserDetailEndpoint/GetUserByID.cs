using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthenticationEndpoints;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
     
    public class GetUserByID : BaseAsyncEndpoint
        .WithRequest<UsersRequest>
        .WithResponse<UsersResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private IUserSession _userAuthentication;

        public ApplicationUser UserInfo { get; set; } = new ApplicationUser();
        public GetUserByID( UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService, IUserSession userAuthentication)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
        }
        
        [SwaggerOperation(
            Summary = "Get a User by Id",
            Description = "Gets a user by Id",
            OperationId = "users.getByID",
            Tags = new[] { "ManagerEndpoints" })
        ]
        [HttpGet("api/users/{UserID}")]
        public override async Task<ActionResult<UsersResponse>> HandleAsync([FromRoute] UsersRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new UsersResponse(request.CorrelationId());

            var user = await  _userAuthentication.GetCurrentSessionUser();
            if(user == null)
                return Unauthorized();
            
            UserInfo.OwnerID = user.Id.ToString();
            
            if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var userGet = await _userManager.FindByIdAsync(request.UserID);
                if (userGet is null) return NotFound();

                var userAndRole = new UserAndRole();
                userAndRole.ImsUser = userGet;
                userAndRole.UserRole = (await _userManager.GetRolesAsync(user))[0];
                return Ok(response);
            }
            return Unauthorized();
        }
    }
}