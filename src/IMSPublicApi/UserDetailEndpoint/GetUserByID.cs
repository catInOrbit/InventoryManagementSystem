using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
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
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class GetUserByID : BaseAsyncEndpoint
        .WithRequest<UsersRequest>
        .WithResponse<UsersResponse>
    {
        private readonly IAsyncRepository<UserInfo> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public UserInfo UserInfo { get; set; } = new UserInfo();
        public GetUserByID(IAsyncRepository<UserInfo> itemRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            _userRepository = itemRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }
        
        [SwaggerOperation(
            Summary = "Get a User by Id",
            Description = "Gets a user by Id",
            OperationId = "users.getByID",
            Tags = new[] { "UserDetailEndpoint" })
        ]
        [HttpGet("api/users/{UserID}")]
        public override async Task<ActionResult<UsersResponse>> HandleAsync([FromRoute] UsersRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new UsersResponse(request.CorrelationId());

            var user = await _userManager.GetUserAsync(HttpContext.User);
            
            UserInfo.OwnerID = user.Id.ToString();
            // requires using ContactManager.Authorization;
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, UserInfo,
                UserOperations.Read);

            if (isAuthorized.Succeeded)
            {
                var userGet = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
                if (userGet is null) return NotFound();

                response.ImsUser = new List<UserInfo>
                {
                    new UserInfo
                    {
                        Id = userGet.Id,
                        OwnerID = userGet.OwnerID,
                        Email = user.UserName,
                        Username = user.UserName,
                        Fullname = userGet.Fullname,
                        Address = userGet.Address,
                        IsActive = userGet.IsActive,
                        PhoneNumber = userGet.PhoneNumber,
                        DateOfBirth = userGet.DateOfBirth
                    }
                };
                return Ok(response); 
            }
            return Unauthorized();
        }
    }
}