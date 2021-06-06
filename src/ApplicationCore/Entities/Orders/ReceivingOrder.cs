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
            Transaction.TransactionId = Id;
            Transaction.TransactionNumber =  DateTime.UtcNow.Date.ToString("ddMMyyyy") +
                           Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            ReceivedDate = DateTime.Now;
            Transaction.Type = TransactionType.Receive;
        }
        
        public string PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public DateTime ReceivedDate { get; set; }
        
        public string SupplierId { get; set; }
        public virtual  Supplier Supplier { get; set; }
        
        public string SupplierInvoice { get; set; }
        public string WarehouseLocation { get; set; }
        public virtual List<ReceivedOrderItem> ReceivedOrderItems { get; set; } = new List<ReceivedOrderItem>();
        public string TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        
    }

}