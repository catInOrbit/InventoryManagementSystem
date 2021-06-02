using System.Text.Json.Serialization;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class VariantValue : BaseEntity
    {
        [JsonIgnore]
        public string ProductVariantId { get; set; }
        // [JsonIgnore]
        // public virtual ProductVariant ProductVariant { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
}