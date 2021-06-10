﻿using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class ProductSearchIndex : BaseEntity
    {
        public ProductSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }

        public bool ShouldSerializeId()
        {
            return false;
        }
        
        public string ProductId { get; set; }
        public string VariantId { get; set; }
        public string Name { get; set; }
        public string Catagory { get; set; }
        public int Quantity { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}