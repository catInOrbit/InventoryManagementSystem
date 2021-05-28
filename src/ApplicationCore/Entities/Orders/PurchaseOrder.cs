using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PurchaseOrder : BaseEntity
    {

        public PurchaseOrder()
        {
            PurchaseOrderNumber = DateTime.UtcNow.Date.ToString("yyyyMMdd") +
                                  Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            purchaseOrderStatus = PurchaseOrderStatus.Draft;
            DateCreated = DateTime.Now;
        }
        
        [Required]
        public string PurchaseOrderNumber { get; set; }
        
        public DateTime DateCreated { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string DeliveryAddress { get; set; }

        public string Description { get; set; }

        [Required]
        public string WarehouseLocation { get; set; }

        [Required]
        public string SupplierId { get; set; }
        
        [Required]
        public string CreatedById { get; set; }

        public string CreatedByName { get; set; }

        public Supplier Supplier { get; set; }

        public PurchaseOrderStatus purchaseOrderStatus { get; set; }

        public decimal totalDiscountAmount { get; set; }

        public decimal totalOrderAmount { get; set; }

        public string purchaseReceiveNumber { get; set; }
        
        public List<PurchaseOrderItemInfo> PurchaseOrderProduct { get; set; } = new List<PurchaseOrderItemInfo>();

    }
}
