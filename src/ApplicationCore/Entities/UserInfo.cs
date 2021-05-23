using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class UserInfo : BaseEntity
    {
        public string OwnerID { get; set; }
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        
        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsActive { get; set; }
    }
}