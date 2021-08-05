using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IUserSession
    {
        void InvalidateSession();
        Task SaveUserAsync(ApplicationUser userGet, string role);
        Task RemoveUserAsync(ApplicationUser user);

        Task<ApplicationUser> GetCurrentSessionUser();
        Task<string> GetCurrentSessionUserRole();
        
        Task<bool> SaveUserResourceAccess(string userId, string resourceId);
        Task<bool> RemoveUserFromResource(string userId, string resourceId);

        Task<Dictionary<string,string>> GetUserResourceDictionary();
    }
}