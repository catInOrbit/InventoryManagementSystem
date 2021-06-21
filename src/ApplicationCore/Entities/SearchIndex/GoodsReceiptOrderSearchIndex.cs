using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class GoodsReceiptOrderSearchIndex : BaseEntity
    {

        public GoodsReceiptOrderSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        public string PurchaseOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SupplierName { get; set; }
        public string CreatedBy { get; set; }
    }
}