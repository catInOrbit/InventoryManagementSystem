using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PurchaseOrderItemInfo : BaseEntity
    {
        public string PurchaseOrderId { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        public string ProductId { get; set; }

        public Product Product { get; set; }

        public float Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}