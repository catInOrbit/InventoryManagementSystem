using System.Threading.Tasks;
using Infrastructure.Identity.Models;
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
        
        public ApplicationUser GetCurrentSessionUser()
        {
            return currentLoggedIn;
        }
        
        public void InvalidateSession()
        {
            _signInManager.SignOutAsync();
        }

        public void SaveUserAsync(ApplicationUser userGet)
        {
          
            currentLoggedIn = userGet;
        }
    }
}