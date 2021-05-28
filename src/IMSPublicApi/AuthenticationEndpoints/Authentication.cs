using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Cors;

namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    [AllowAnonymous]
    public class Authentication : BaseAsyncEndpoint
        .WithRequest<AuthenticateRequest>
        .WithResponse<AuthenticateResponse>
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenClaimsService _tokenClaimsService;

        private readonly IAsyncRepository<UserInfo> _userRepository;
        private readonly UserRoleModificationService _userRoleModificationService;
        public UserInfoAuth UserInfo { get; set; } = new UserInfoAuth();

        public Authentication(IAsyncRepository<UserInfo> userRepository, SignInManager<ApplicationUser> signInManager,
            ITokenClaimsService tokenClaimsService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _tokenClaimsService = tokenClaimsService;
            _userRepository = userRepository;
            _userRoleModificationService = new UserRoleModificationService(roleManager, userManager);
        }

        [HttpPost("api/authentication")]
        [SwaggerOperation(
            Summary = "Authenticates a user",
            Description = "Authenticates a user",
            OperationId = "auth.authenticate",
            Tags = new[] { "IMSAuthenticationEndpoints" })
        ]
        public override async Task<ActionResult<AuthenticateResponse>> HandleAsync(AuthenticateRequest request, CancellationToken cancellationToken)
        {
            var response = new AuthenticateResponse(request.CorrelationId());
            
            var user = await _userRoleModificationService.UserManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.Result = false;
                response.Verbose = "Can not find user with username" + request.Email;
                return Unauthorized(response);
            }

            else
            {
                var roles = await _userRoleModificationService.UserManager.GetRolesAsync(user);
                var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, true, true);

                if (result.Succeeded)
                {
                    // await HttpContext.AuthenticateAsync("Cookie", userPrincipal);
                    var jwttoken = await _tokenClaimsService.GetTokenAsync(user.Email);
                    // Write the login id in the login claim, so we identify the login context
                    // Claim[] customClaims = { new Claim("UserLoginSessionId", token) };

                    // await _userManager.AddClaimsAsync(user,  customClaims);
                    // Signin User
                    // CookieSignInAndStore(user.Email, user.UserName, roles[0]);

                    // await _signInManager.SignInWithClaimsAsync(user, true, customClaims);

                    response.Token = jwttoken;
                    response.Verbose = "Success";
                    var userGet = await _userRepository.GetByIdAsync(user.Id, cancellationToken);
                    var claims = await _userRoleModificationService.ClaimGettingHelper();
                    var userPrinciple = new GenericPrincipal(new ClaimsIdentity(userGet.Username), roles.ToArray());
                    HttpContext.User = userPrinciple;

                    var claimListDistince = claims.Select(x => x.Type).Distinct();

                    response.PageAuthorized = claimListDistince.ToList();
                    response.Result = result.Succeeded;
                    response.IsLockedOut = result.IsLockedOut;
                    response.IsNotAllowed = result.IsNotAllowed;
                    response.RequiresTwoFactor = result.RequiresTwoFactor;
                    if (userGet != null)
                        response.Username = userGet.Fullname;
                    response.UserRole = roles[0];
                    response.UserInfo = userGet;
                    return Ok(response);

                }

            }
                    return Unauthorized(response);

        }

        private async void CookieSignInAndStore(string email, string fullname, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim("FullName", fullname),
                new Claim(ClaimTypes.Role, role),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);
        }


    }
}
