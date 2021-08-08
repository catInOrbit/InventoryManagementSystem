using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class RODisplay
    {
        public string GoodsReceiptId { get; set; }
        public string PurchaseOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SupplierName { get; set; }

    }
}