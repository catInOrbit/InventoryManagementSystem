using System.Collections.Generic;
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
        
        Task<bool> SaveUserResourceAccess(string userId, string resourceId);
        Task<bool> RemoveUserFromResource(string userId, string resourceId);

        Task<Dictionary<string,string>> GetUserResourceDictionary();

    }
}