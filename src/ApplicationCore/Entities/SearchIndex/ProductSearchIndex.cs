using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class ProductSearchIndex : BaseEntity
    {
        public ProductSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        public string ProductId { get; set; }
        public string VariantId { get; set; }
        public string Name { get; set; }
    }
}