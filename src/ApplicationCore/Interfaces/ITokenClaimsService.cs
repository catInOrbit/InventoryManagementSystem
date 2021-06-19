using System.Threading.Tasks;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface ITokenClaimsService
    {
        Task<string> GetTokenAsync(string email);
        Task<string> GetRefreshTokenAsync(string email);
    }
}
