using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Encodings;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
    public class GetAllUsers : BaseAsyncEndpoint.WithoutRequest.WithResponse<UsersResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private IUserAuthentication _userAuthentication;

        public ApplicationUser UserInfo { get; set; } = new ApplicationUser();

        public GetAllUsers(UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService, IUserAuthentication userAuthentication)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
        }

        [HttpGet("api/users")]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Gets all users",
            OperationId = "users.GetAll",
            Tags = new[] { "ManagerEndpoints" })
        ]
        public override async Task<ActionResult<UsersResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new UsersResponse();
            
            var user = await _userAuthentication.GetCurrentSessionUser();
            if(user == null)
                return Unauthorized();
            
            UserInfo.OwnerID = user.Id.ToString();

            if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var users = _userManager.Users.Where(user => user.IsActive == true);
                response.ImsUser = users.ToList();
                return Ok(response);
            }

            return Unauthorized();
        }
    }
}