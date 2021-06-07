using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PurchaseOrder : BaseEntity
    {
        public PurchaseOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
            // Transaction.TransactionId = Id;
            // Transaction.TransactionNumber =  DateTime.UtcNow.Date.ToString("ddMMyyyy") +
            //                Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            PurchaseOrderStatus = PurchaseOrderStatusType.Created;
            // Transaction.CreatedDate = DateTime.Now;
            // Transaction.Type = TransactionType.Purchase;
        }
        public virtual bool ShouldSerializePurchaseOrderProduct()
        {
            return true;
        }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string MailDescription { get; set; }
        public string SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string WarehouseLocation { get; set; }
        public PurchaseOrderStatusType PurchaseOrderStatus { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderProduct { get; set; } = new List<PurchaseOrderItem>();
        
        public string PriceQuoteOrderId { get; set; }
        public virtual PriceQuoteOrder PriceQuoteOrder { get; set; }
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        
    }
}
