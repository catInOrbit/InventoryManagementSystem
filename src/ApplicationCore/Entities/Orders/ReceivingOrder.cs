using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class ReceivingOrder : BaseEntity
    {

        public ReceivingOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
            ReceiveOrderNumber =  DateTime.UtcNow.Date.ToString("ddMMyyyy") +
                                  Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            ReceivedDate = DateTime.Now;
            Type = TransactionType.Receive;
        }
        
        public string PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public string ReceiveOrderNumber { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CreatedById { get; set; }
        public virtual UserInfo CreatedBy { get; set; }
        public string SupplierId { get; set; }
        public virtual  Supplier Supplier { get; set; }
        public string SupplierInvoice { get; set; }
        public string BranchId { get; set; }
        public string WarehouseLocation { get; set; }
        public virtual List<ReceivedOrderItem> ReceivedOrderItems { get; set; } = new List<ReceivedOrderItem>();
        public TransactionType Type { get; set; }
    }

}