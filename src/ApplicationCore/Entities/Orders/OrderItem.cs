using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class OrderItem : BaseEntity
    {
        public OrderItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        [JsonIgnore]
        public string OrderId { get; set; }
        public string ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        public int OrderQuantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}