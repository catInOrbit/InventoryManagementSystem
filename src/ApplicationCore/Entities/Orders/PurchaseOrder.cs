using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PurchaseOrder : BaseEntity
    {
        public PurchaseOrder()
        {
            Id = "PO" +Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            // Transaction.TransactionId = Id;
            // Transaction.TransactionNumber =  DateTime.UtcNow.Date.ToString("ddMMyyyy") +
            //                Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            PurchaseOrderStatus = PurchaseOrderStatusType.PriceQuote;
            // Transaction.CreatedDate = DateTime.Now;
            // Transaction.Type = TransactionType.Purchase;
        }
        public virtual bool ShouldSerializePurchaseOrderProduct()
        {
            return true;
        }
        
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string MergedWithPurchaseOrderId { get; set; }
        public string MailDescription { get; set; }
        public string SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string WarehouseLocation { get; set; }
        public PurchaseOrderStatusType PurchaseOrderStatus { get; set; }
        [NotMapped]
        public string PurchaseOrderStatusString { get; set; }
        
        [Column(TypeName = "decimal(16,3)")]
        public decimal TotalDiscountAmount { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal TotalOrderAmount { get; set; }

        public int TotalProductAmount { get; set; } = 0;
        public virtual ICollection<OrderItem> PurchaseOrderProduct { get; set; } = new List<OrderItem>();
        public string TransactionId { get; set; }
        public DateTime Deadline { get; set; }
        
        public virtual Transaction Transaction { get; set; }
        public bool HasSentMail { get; set; }
          
        [OnSerializing]
        public void FormatResponse(StreamingContext context)
        {
            TotalProductAmount = 0;
            PurchaseOrderStatusString = PurchaseOrderStatus.ToString();
            foreach (var orderItem in PurchaseOrderProduct)
            {
                orderItem.ProductVariant.IsShowingTransaction = false;
                foreach (var productVariantPackage in orderItem.ProductVariant.Packages)
                    productVariantPackage.IsShowingTransaction = false;
                
                TotalProductAmount += 1;
            }
        }
        
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
