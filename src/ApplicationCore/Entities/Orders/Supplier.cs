using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nest;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{

    public class Supplier : BaseEntity
    {
        public Supplier()
        {
            Id = "SPL" +Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        [StringLength(50)] [Required] public string SupplierName { get; set; }

         public string Description { get; set; }
         public string Address { get; set; }
         public string SalePersonName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string TransactionId { get; set; }
        [JsonIgnore]
        [Ignore]
        public virtual Transaction  Transaction{ get; set; }
        //IBaseAddress
    }
}
