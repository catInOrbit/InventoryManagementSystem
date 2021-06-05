using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class ReceivingOrderSearchIndex : BaseEntity
    {

        public ReceivingOrderSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        public string receiptId { get; set; }
        public string purchaseOrderId { get; set; }
        public string createdDate { get; set; }
        public string supplierName { get; set; }
        public string createdBy { get; set; }
    }
}