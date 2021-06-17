using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class GoodsReceiptOrder : BaseEntity
    {

        public GoodsReceiptOrder()
        {
            Id = "GR" +Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                           Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            ReceivedDate = DateTime.Now;
        }
        public string PurchaseOrderId { get; set; }
        [JsonIgnore]
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public string StorageLocationReceipt { get; set; }
        public DateTime ReceivedDate { get; set; }
        
        [JsonIgnore]
        public string SupplierId { get; set; }
        public virtual  Supplier Supplier { get; set; }
        
        public string SupplierInvoice { get; set; }
        public virtual List<GoodsReceiptOrderItem> ReceivedOrderItems { get; set; } = new List<GoodsReceiptOrderItem>();
        [JsonIgnore]
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        
    }

}