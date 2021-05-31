using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class OrderItemInfo : BaseEntity
    {
        public string PurchaseOrderId { get; set; }
        [JsonIgnore]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public string ProductId { get; set; }

        public virtual Product Product { get; set; }

        public float Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}