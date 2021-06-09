using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore]
        public string OwnerID { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthNormalizedString { get; set; }
        public bool IsActive { get; set; }
    }
}