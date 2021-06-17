
using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class VariantValue : BaseEntity
    {
        public VariantValue()
        {
            Id = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
        }
        public string ProductVariantId { get; set; }
        // [JsonIgnore]
        // public virtual ProductVariant ProductVariant { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
}