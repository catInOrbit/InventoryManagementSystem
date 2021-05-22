using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthenticationEndpoints;
using InventoryManagementSystem.PublicApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
    [Authorize]
    public class GetUserByID : BaseAsyncEndpoint
        .WithRequest<UsersRequest>
        .WithResponse<UsersResponse>
    {
        private readonly IAsyncRepository<IMSUser> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public IMSUser IMSUser { get; set; } = new IMSUser();
        public GetUserByID(IAsyncRepository<IMSUser> itemRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
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
            
            IMSUser.OwnerID = user.Id.ToString();
            // requires using ContactManager.Authorization;
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, IMSUser,
                UserOperations.Read);

            if (isAuthorized.Succeeded)
            {
                var userGet = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
                if (userGet is null) return NotFound();

                response.ImsUser = new List<IMSUser>
                {
                    new IMSUser
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