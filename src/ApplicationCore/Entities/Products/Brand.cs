using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Brand : BaseEntity
    {
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        
        public virtual ICollection<Products.Product> Product { get; set; }

    }
}