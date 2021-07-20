using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool ShouldSerializePasswordHash()
        {
            return false;
        }
        
        public bool ShouldSerializeSecurityStamp()
        {
            return false;
        }
        
        public bool ShouldSerializeConcurrencyStamp()
        {
            return false;
        }
        
        [JsonIgnore]
        public string OwnerID { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthNormalizedString { get; set; }
        public string ProfileImageLink { get; set; }
        public bool IsActive { get; set; }
    }
    
    // public class ApplicationRole : IdentityRole
    // {
    //     // public string RoleDescription { get; set; }
    // }
}