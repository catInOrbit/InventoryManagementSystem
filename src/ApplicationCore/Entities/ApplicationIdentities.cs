using System;
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
        
        public bool ShouldSerializePhoneNumberConfirmed()
        {
            return false;
        }
        
        public bool ShouldSerializeTwoFactorEnabled()
        {
            return false;
        }
        
        public bool ShouldSerializeLockOutEnd()
        {
            return false;
        }
        
        public bool ShouldSerializeLockoutEnabled()
        {
            return false;
        }
        
        public bool ShouldSerializeAccessFailedCount()
        {
            return false;
        }
        
        public bool ShouldSerializeEmailConfirmed()
        {
            return false;
        }
        
        public bool ShouldSerializeLockoutEnd()
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
    
    public class UserAndRole
    {
        public ApplicationUser ImsUser { get; set; }
        public string UserRole { get; set; }
        public string RoleID { get; set; }
    }
    
    // public class ApplicationRole : IdentityRole
    // {
    //     // public string RoleDescription { get; set; }
    // }
}