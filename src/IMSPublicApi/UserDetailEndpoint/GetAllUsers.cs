using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Encodings;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
    [Authorize]
    public class GetAllUsers : BaseAsyncEndpoint.WithoutRequest.WithResponse<UsersResponse>
    {
        private readonly IAsyncRepository<IMSUser> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public IMSUser IMSUser { get; set; } = new IMSUser();

        public GetAllUsers(IAsyncRepository<IMSUser> userRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/users")]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Gets all users",
            OperationId = "users.GetAll",
            Tags = new[] { "UserDetailEndpoint" })
        ]
        public override async Task<ActionResult<UsersResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new UsersResponse();
            
            var user = await _userManager.GetUserAsync(HttpContext.User);
            
            IMSUser.OwnerID = user.Id.ToString();
            // requires using ContactManager.Authorization;
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, IMSUser,
                UserOperations.Read);

            if (isAuthorized.Succeeded)
            {
                var users = await _userRepository.ListAllAsync();
                response.ImsUser = (List<IMSUser>) users;
                return Ok(response);
                
            }
            return Unauthorized();
        }
    }
}