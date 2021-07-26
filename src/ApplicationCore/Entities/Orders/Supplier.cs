using System;
using System.ComponentModel.DataAnnotations;
using Nest;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{

    public class Supplier : BaseSearchIndex
    {
        public Supplier()
        {
            Id = "SPL" +Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        [StringLength(50)] [Required] public string SupplierName { get; set; }
            
        
        [PropertyName("description")]
         public string Description { get; set; }
         [PropertyName("address")]
         public string Address { get; set; }
         [PropertyName("salePersonName")]
         public string SalePersonName { get; set; }
         [PropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
        [PropertyName("email")]
        public string Email { get; set; }
        [PropertyName("transactionId")]

        public string TransactionId { get; set; }
        
        [JsonIgnore]
        [Ignore]
        public virtual Transaction  Transaction{ get; set; }

    }
}
