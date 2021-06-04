using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class PurchaseOrderSearchIndex : BaseEntity
    {
        public PurchaseOrderSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }

        public string PurchaseOrderNumber { get; set; }
        public string SupplierName { get; set; }
        public string ConfirmedByName { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}