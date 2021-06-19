using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace Infrastructure.Services
{
    public interface IUserAuthentication
    {
        void InvalidateSession();
        Task SaveUserAsync(ApplicationUser userGet);

        Task<ApplicationUser> GetCurrentSessionUser();
    }
}