using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Product : BaseEntity
    {
        public Product()
        {
            Id = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
        }
        
        //TODO:JSON IGNORE NOT WORKING
        
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string CategoryId { get; set; }
        [JsonIgnore]
        public virtual Transaction Transaction { get; set; }
        public string SellingStrategy { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        public bool IsVariantType { get; set; }
    }
}
