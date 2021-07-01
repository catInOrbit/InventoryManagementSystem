using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Brand : BaseEntity
    {
        public Brand()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}