using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class StockTakeSearchIndex : BaseEntity
    {
        public StockTakeSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        
        public string  TransactionId { get; set; }
        public string CreatedByName { get; set; }
        public string Status { get; set; }
        
        public string DeliveryMethod { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}