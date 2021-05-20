using System.Threading.Tasks;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface ITokenClaimsService
    {
        Task<string> GetTokenAsync(string userName);
    }
}
