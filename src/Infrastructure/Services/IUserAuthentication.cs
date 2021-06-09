using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace Infrastructure.Services
{
    public interface IUserAuthentication
    {
        Task<ApplicationUser> GetById(string id);
        void InvalidateSession();
        Task SaveUserAsync(ApplicationUser userGet);
        Task<ApplicationUser> GetCurrentSessionUser();
    }
}