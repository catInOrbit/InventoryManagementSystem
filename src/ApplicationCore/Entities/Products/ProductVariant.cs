using System;
using System.Collections;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Nest;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class ProductVariant : BaseEntity
    {
        public ProductVariant()
        {
            Id = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
        }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string Sku { get; set; }
        public string Unit { get; set; }
        public int StorageQuantity { get; set; }
        public string TransactionId { get; set; }
        
        [JsonIgnore]
        public virtual Transaction Transaction { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [Nest.PropertyName("variantValues")]
        [Nest.Nested]
        // public virtual ICollection<VariantValue> VariantValues { get; set; }
        public bool IsVariantType { get; set; }
        public virtual IList<Package> Packages { get; set; }
    }
}