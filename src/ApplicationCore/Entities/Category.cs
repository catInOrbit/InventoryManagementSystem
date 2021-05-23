using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class Category : BaseEntity
    {
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}