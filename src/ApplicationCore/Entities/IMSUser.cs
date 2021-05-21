using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class IMSUser : BaseEntity
    {
        public string OwnerID { get; set; }
        
        public string Id { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        
        public string Address { get; set; }

        public bool IsActive { get; set; }
        
    }
}