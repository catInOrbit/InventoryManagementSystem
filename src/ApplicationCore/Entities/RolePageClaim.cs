using Microsoft.AspNetCore.Identity;
namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class RolePageClaim : BaseEntity
    {
        public string Id { get; set; }
        public string Page { get; set; }
    }
}