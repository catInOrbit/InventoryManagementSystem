using System;

namespace InventoryManagementSystem.ApplicationCore.Extensions
{
    public class MergedOrderIdList
    {
        public string ParentOrderId { get; set; }
        public string PurchaseOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}