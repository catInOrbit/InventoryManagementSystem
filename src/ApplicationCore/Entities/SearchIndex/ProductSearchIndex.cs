using System;
using System.Collections.Generic;
using Nest;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class ProductVariantSearchIndex : BaseEntity
    {
        public ProductVariantSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }

        public string  TransactionId { get; set; }

        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }

        public string Category { get; set; }
        public string Strategy { get; set; }
        public string Brand { get; set; }
        public string ProductId { get; set; }
        public string ProductVariantId { get; set; }
        public string SupplierName { get; set; }
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
    
    public class ProductSearchIndex : BaseEntity
    {
        public override string Id { get; set; }

        public ProductSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }

        public string  TransactionId { get; set; }

        public string Name { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string Category { get; set; }
        public string Strategy { get; set; }
        public string Brand { get; set; }
        public string ProductId { get; set; }
        public bool  IsVariantType { get; set; }
        public List<string> VariantIds { get; set; } = new List<string>();
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