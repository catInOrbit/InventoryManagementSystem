using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class GoodsReceiptOrder : BaseEntity
    {

        public GoodsReceiptOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
            // Transaction.TransactionId = Id;
            // Transaction.TransactionNumber =  DateTime.UtcNow.Date.ToString("ddMMyyyy") +
                           Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            ReceivedDate = DateTime.Now;
            // Transaction.Type = TransactionType.Receive;
        }
        
        public string PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public DateTime ReceivedDate { get; set; }
        
        public string SupplierId { get; set; }
        public virtual  Supplier Supplier { get; set; }
        
        public string SupplierInvoice { get; set; }
        public string WarehouseLocation { get; set; }
        public virtual List<GoodsReceiptOrderItem> ReceivedOrderItems { get; set; } = new List<GoodsReceiptOrderItem>();
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        
    }

}