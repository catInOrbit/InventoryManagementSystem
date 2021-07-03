using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        public virtual ICollection<Product> Products { get; set; }

        public bool ShouldSerializeProducts()
        {
            if (!IsShowingProducts)
                return false;
            return true;
        }
        
        [JsonIgnore]
        [NotMapped]
        public bool IsShowingProducts { get; set; }
    }
}