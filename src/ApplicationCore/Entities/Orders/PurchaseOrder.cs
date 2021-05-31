using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PurchaseOrder : BaseEntity
    {

        public PurchaseOrder()
        {
            Id = DateTime.UtcNow + "-"+Guid.NewGuid().ToString();
            PurchaseOrderNumber = (PriceQuoteOrderId == null) ?  DateTime.UtcNow.Date.ToString("yyyyMMdd") +
                                  Guid.NewGuid().ToString().Substring(0, 5).ToUpper() : PriceQuoteOrderId;
            PurchaseOrderStatus = PurchaseOrderStatusType.Draft;
            CreatedDate = DateTime.Now;
            Type = TransactionType.Purchase;
        }
        
        public DateTime CreatedDate { get; set; }
        
        [Required]
        public string PurchaseOrderNumber { get; set; }
        
        public DateTime DeliveryDate { get; set; }

        public string DeliveryAddress { get; set; }

        public string Description { get; set; }
        
        [Required]
        public string SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        [Required]
        public string WarehouseLocation { get; set; }
        
        [Required]
        public string CreatedById { get; set; }

        public string CreatedByName { get; set; }
        public string PriceQuoteOrderId { get; set; }

        public PurchaseOrderStatusType PurchaseOrderStatus { get; set; }
        public TransactionType Type { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal TotalOrderAmount { get; set; }

        public string PurchaseReceiveNumber { get; set; }
        
        public virtual ICollection<OrderItemInfo> PurchaseOrderProduct { get; set; } = new List<OrderItemInfo>();
        public virtual PriceQuoteOrder PriceQuoteOrder { get; set; }
        public virtual UserInfo CreatedBy { get; set; }

    }
}
