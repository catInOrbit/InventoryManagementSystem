using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Nest;

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
        [Ignore]
        public virtual Transaction Transaction { get; set; }
        [JsonIgnore]
        [Ignore]
        public virtual Product Product { get; set; }
        // public virtual ICollection<VariantValue> VariantValues { get; set; }
        public bool IsVariantType { get; set; }
        [Ignore]
        public virtual IList<Package> Packages { get; set; }

        [Ignore]
        [JsonIgnore]
        [NotMapped]
        public bool IsShowingTransaction { get; set; }
        [Ignore] [JsonIgnore] [NotMapped] public bool IsShowingPackage { get; set; } = true;
        public bool ShouldSerializeTransaction()
        {
            if(IsShowingTransaction)
                return true;
            return false;
        }

        public bool ShouldSerializePackages()
        {
            if (IsShowingPackage)
                return true;
            return false;
        }

        public bool ShouldSerializeProduct()
        {
            return false;
        }
    }
}