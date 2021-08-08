using System;
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
        public int QuantityLeftAfterReceived { get; set; }
        public string Unit { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal Price { get; set; }
        [JsonIgnore]
        [Column(TypeName = "decimal(16,3)")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal TotalAmount { get; set; }


        [NotMapped] [Ignore] [JsonIgnore] public bool IsShowingProductVariant { get; set; } = false;
        [NotMapped] [Ignore] [JsonIgnore] public bool IsShowingProductVariantDetail { get; set; } = false;

        [NotMapped] [Ignore] [JsonIgnore] public bool IsShowing { get; set; } = false;

        public bool ShouldSerializeProductVariant()
        {
            if (IsShowingProductVariant)
                return true;
            return false;
        }

        [OnSerializing]
        public void FormatProductVariantResponse(StreamingContext context)
        {
            if (!IsShowingProductVariantDetail)
            {
                ProductVariant.IsShowingTransaction = false;
                ProductVariant.IsShowingPackage = false;
            }

            else
            {
                ProductVariant.IsShowingTransaction = false;
                ProductVariant.IsShowingPackage = true;
            }
                
        }
    }
}