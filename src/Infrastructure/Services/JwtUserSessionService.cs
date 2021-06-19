using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class JwtUserSessionService : IUserAuthentication
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private ITokenClaimsService _tokenClaimsService;

        private ApplicationUser currentLoggedIn = new ApplicationUser();

        public JwtUserSessionService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenClaimsService tokenClaimsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenClaimsService = tokenClaimsService;
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            var returnUser = await _userManager.FindByIdAsync(id);
            return returnUser;
        }


        public async Task<string> GenerateRefreshTokenForUser(ApplicationUser user)
        {
            var tokenRefresh = await _tokenClaimsService.GetRefreshTokenAsync(user.Email);
            if (await _userManager.GetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken") != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken");
                await _userManager.SetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken", tokenRefresh);
            }
            else
                await _userManager.SetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken", tokenRefresh);

            return tokenRefresh;
        }

        public async Task<string> GetTokenRefreshOfUser(ApplicationUser user)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken");
        }

        public async Task<ApplicationUser> GetCurrentSessionUser()
        {
            return await Task.FromResult(currentLoggedIn);
        }
        
        public void InvalidateSession()
        {
            _signInManager.SignOutAsync();
        }

        public Task SaveUserAsync(ApplicationUser userGet)
        {
            return Task.FromResult(currentLoggedIn = userGet);
        }
    }
}