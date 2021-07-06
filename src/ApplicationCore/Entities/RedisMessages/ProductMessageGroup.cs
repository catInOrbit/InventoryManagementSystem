using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities.RedisMessages
{
    public class ProductMessageGroup
    {
        public string Id { get; set; }
        public DateTime ModifiedDate { get; set; }

        public List<ProductUpdateMessage> ProductUpdateMessages { get; set; } = new List<ProductUpdateMessage>();
    }
}