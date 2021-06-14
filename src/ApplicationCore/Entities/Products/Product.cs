using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Product : BaseEntity
    {
        public Product()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string CategoryId { get; set; }

        public Transaction Transaction { get; set; }
        public string SellingStrategy { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        public bool IsVariantType { get; set; }
    }
}