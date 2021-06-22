using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class UserSessionService : IUserSession
    {

        private ApplicationUser currentLoggedIn = new ApplicationUser();
        private string CurrentUserRole { get; set; }

        public async Task<ApplicationUser> GetCurrentSessionUser()
        {
            return await Task.FromResult(currentLoggedIn);
        }

        public async Task<string> GetCurrentSessionUserRole()
        {
            return await Task.FromResult(CurrentUserRole);
        }

        public void InvalidateSession()
        {
            // _signInManager.SignOutAsync();
        }

        public Task SaveUserAsync(ApplicationUser userGet, string role)
        {
            Task.FromResult(CurrentUserRole = role);
            return Task.FromResult(currentLoggedIn = userGet);
        }
    }
}