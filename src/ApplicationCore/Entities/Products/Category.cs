using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Category : BaseEntity
    {
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}