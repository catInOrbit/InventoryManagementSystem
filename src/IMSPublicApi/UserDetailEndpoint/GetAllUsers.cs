using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
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
        private readonly IAsyncRepository<UserInfo> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private IUserAuthentication _userAuthentication;

        public UserInfo UserInfo { get; set; } = new UserInfo();

        public GetAllUsers(IAsyncRepository<UserInfo> userRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService, IUserAuthentication userAuthentication)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
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
            
            var user = await _userAuthentication.GetCurrentSessionUser();
            if(user == null)
                return Unauthorized();
            
            UserInfo.OwnerID = user.Id.ToString();

            if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var users = await _userRepository.ListAllAsync();
                response.ImsUser = (List<UserInfo>) users;
                return Ok(response);
            }

            // requires using ContactManager.Authorization;
            // var isAuthorized = await _authorizationService.AuthorizeAsync(
            //     HttpContext.User, UserInfo,
            //     UserOperations.Read);
            
            // if (isAuthorized.Succeeded)
            // {
            //     var users = await _userRepository.ListAllAsync();
            //     response.ImsUser = (List<UserInfo>) users;
            //     return Ok(response);
            // }
            return Unauthorized();
        }
    }
}