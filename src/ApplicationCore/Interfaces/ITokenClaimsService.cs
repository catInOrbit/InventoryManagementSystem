using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface ITokenClaimsService
    {
        Task<string> GetTokenAsync(string email);
        Task<string> GenerateRefreshTokenAsync(string email);
        
        Task<string> GetRefreshTokenAsync(ApplicationUser user);

        Task SaveRefreshTokenForUser(ApplicationUser user, string tokenRefresh);
    }
}
