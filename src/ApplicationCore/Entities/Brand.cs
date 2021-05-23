using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class Brand : BaseEntity
    {
        public string Id { get; set; }
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        
        public ICollection<Product> Product { get; set; }

    }
}