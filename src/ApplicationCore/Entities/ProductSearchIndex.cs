using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class ProductSearchIndex : BaseEntity
    {
        public string ProductId { get; set; }
        [Nest.PropertyName("name")]
        public string Name { get; set; }
        [Nest.PropertyName("price")]
        public decimal Price { get; set; }
        [Nest.PropertyName("variantValues")]
        public List<VariantValue> VariantValues { get; set; }
        [Nest.PropertyName("serialNumbers")]
        [Nest.Nested]
        public virtual List<ProductSerialNumber> SerialNumbers { get; set; }
    }
}