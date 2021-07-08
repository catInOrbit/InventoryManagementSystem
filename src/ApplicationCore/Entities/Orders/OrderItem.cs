using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Nest;

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
        [JsonIgnore]
        public virtual ProductVariant ProductVariant { get; set; }
        public int OrderQuantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore]
        public decimal SalePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }


        [NotMapped] [Ignore] [JsonIgnore] public bool IsShowingProductVariantDetail { get; set; } = false;


        public bool ShouldSerializeProductVariant()
        {
            if (IsShowingProductVariantDetail)
                return true;
            return false;
        }

        [OnSerializing]
        public void FormatProductVariantResponse(StreamingContext context)
        {
            ProductVariant.IsShowingTransaction = false;
            ProductVariant.IsShowingPackage = false;
        }
    }
}