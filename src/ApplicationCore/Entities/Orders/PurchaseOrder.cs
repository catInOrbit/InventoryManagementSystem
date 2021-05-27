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
        }
        
        [StringLength(20)]
        [Required]
        [Display(Name = "Purchase Order Number")]
        public string PurchaseOrderNumber { get; set; }
        
        [Display(Name = "PO Date")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Delivery Date")]
        public DateTime DeliveryDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        [StringLength(100)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [StringLength(38)]
        [Required]
        [Display(Name = "Branch Id")]
        public string WarehouseLocation { get; set; }

        [StringLength(38)]
        [Required]
        [Display(Name = "Vendor Id")]
        public string SupplierId { get; set; }
        
        [StringLength(38)]
        [Required]
        public string CreatedById { get; set; }

        public string CreatedByName { get; set; }

        [Display(Name = "Supplier")]
        public Supplier Supplier { get; set; }

        [Display(Name = "PO Status")]
        public PurchaseOrderStatus purchaseOrderStatus { get; set; }

        [Display(Name = "Total Discount")]
        public decimal totalDiscountAmount { get; set; }

        [Display(Name = "Total Order")]
        public decimal totalOrderAmount { get; set; }

        [Display(Name = "Purchase Receive Number")]
        public string purchaseReceiveNumber { get; set; }
        
        public List<PurchaseOrderProduct> PurchaseOrderProduct { get; set; } = new List<PurchaseOrderProduct>();

    }
}
