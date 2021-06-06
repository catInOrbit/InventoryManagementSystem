using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Nest;

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
        [Nest.Text( Name = "sku")]
        public string Sku { get; set; }
        [Nest.Text( Name = "unit")]
        public string Unit { get; set; }
        [Nest.Number(NumberType.Float, Name = "quantity", IgnoreMalformed = true)]
        public float Quantity { get; set; }
        [Nest.PropertyName("storageLocation")]  
        public string StorageLocation { get; set; }
        [Date(Format = "yyyy-MM-dd")]
        public DateTime CreatedDate { get; set; }
        [Date(Format = "yyyy-MM-dd")]
        public DateTime ModifiedDate { get; set; }
        //  [JsonIgnore]
        // public virtual Product Product { get; set; }
        [Nest.PropertyName("variantValues")]
        [Nest.Nested]
        public virtual ICollection<VariantValue> VariantValues { get; set; }
        [Nest.Nested]
        [Nest.PropertyName("serialNumbers")]
        public virtual ICollection<ProductSerialNumber> SerialNumbers { get; set; }
        [Nest.PropertyName("isVariantType")]
        public bool IsVariantType { get; set; }
    }
}