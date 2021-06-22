using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace Infrastructure.Services
{
    public interface IUserSession
    {
        void InvalidateSession();
        Task SaveUserAsync(ApplicationUser userGet, string role);

        Task<ApplicationUser> GetCurrentSessionUser();
        Task<string> GetCurrentSessionUserRole();
    }
}