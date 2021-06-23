using System;
using System.Collections.Generic;
using Nest;

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
        
        public string  TransactionId { get; set; }

        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }

        public string Category { get; set; }
        public string Strategy { get; set; }
        public string Brand { get; set; }
        public string ProductId { get; set; }
        public string VariantId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public CompletionField Suggest { get; set; }

        public void FillSuggestion()
        {
            List<string> list = new List<string>();
            list.Add(Name);
            
            Suggest = new CompletionField
            {
                Input = list,
                Weight = 1
            };
            
        }
    }
}