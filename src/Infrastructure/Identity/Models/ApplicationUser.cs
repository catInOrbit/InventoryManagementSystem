using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual UserInfo UserInfo { get; set; }
    }
}