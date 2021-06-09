using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class JwtUserSessionService : IUserAuthentication
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        private ApplicationUser currentLoggedIn = new ApplicationUser();

        public JwtUserSessionService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            var returnUser = await _userManager.FindByIdAsync(id);
            return returnUser;
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