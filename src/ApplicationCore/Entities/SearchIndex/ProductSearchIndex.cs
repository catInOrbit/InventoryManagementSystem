using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class ProductSearchIndex : BaseEntity
    {
        public ProductSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        public string productId { get; set; }
        public string variantId { get; set; }
        public string name { get; set; }
    }
}