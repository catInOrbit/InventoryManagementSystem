using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Product : BaseEntity
    {
        
        public string Name { get; set; }
        public string BrandId { get; set; }
        public string CategoryId { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string SellingStrategy { get; set; }
        
        public virtual Category Category { get; set; }
        public virtual Brand Brand { get; set; }
        
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}