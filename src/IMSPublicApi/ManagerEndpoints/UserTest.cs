using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class UserTest : BaseEntity
    {
        public string Username { get; private set; }

        public UserTest(string username)
        {
            Username = username;
        }
    }
    
}