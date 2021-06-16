using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Nest;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class ProductVariant : BaseEntity
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public string Unit { get; set; }
        public int StorageQuantity { get; set; }
        public string StorageLocation { get; set; }
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [Nest.PropertyName("variantValues")]
        [Nest.Nested]
        public virtual ICollection<VariantValue> VariantValues { get; set; }

        public bool IsVariantType { get; set; }
    }
}