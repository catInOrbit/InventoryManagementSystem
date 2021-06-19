using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace Infrastructure.Services
{
    public interface IUserAuthentication
    {
        Task<ApplicationUser> GetById(string id);
        void InvalidateSession();
        Task SaveUserAsync(ApplicationUser userGet);
        Task<string> GenerateRefreshTokenForUser(ApplicationUser user);
        Task<string> GetTokenRefreshOfUser(ApplicationUser user);

        Task<ApplicationUser> GetCurrentSessionUser();
    }
}