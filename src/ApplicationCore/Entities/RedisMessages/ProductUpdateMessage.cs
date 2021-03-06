using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.RedisMessages
{
    public class ProductUpdateMessage
    {
        public string Id { get; set; }
        public string ProductVariantId { get; set; }
        public string ProductVariantName { get; set; }

        public string Sku { get; set; }

        public ProductUpdateMessage()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}