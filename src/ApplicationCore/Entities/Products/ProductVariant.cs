using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class ProductVariant : BaseEntity
    {
        [Nest.PropertyName("productId")]
        public string ProductId { get; set; }
        [Nest.PropertyName("name")]
        public string Name { get; set; }
        [Nest.PropertyName("price")]
        public decimal Price { get; set; }
        [Nest.PropertyName("sku")]
        public string Sku { get; set; }
        [Nest.PropertyName("unit")]
        public string Unit { get; set; }
        [Nest.PropertyName("quantity")]
        public float Quantity { get; set; }
        [Nest.PropertyName("storageLocation")]
        public string StorageLocation { get; set; }
        [Nest.PropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }
        [Nest.PropertyName("modifiedDate")]
        public DateTime ModifiedDate { get; set; }
         [JsonIgnore]
        public virtual Product Product { get; set; }
        [Nest.PropertyName("variantValues")]
        [Nest.Nested]
        public virtual ICollection<VariantValue> VariantValues { get; set; }
        [Nest.Nested]
        [Nest.PropertyName("serialNumbers")]
        public virtual ICollection<ProductSerialNumber> SerialNumbers { get; set; }
    }
}