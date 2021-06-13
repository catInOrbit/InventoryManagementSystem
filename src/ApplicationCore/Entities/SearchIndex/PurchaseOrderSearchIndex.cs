using System;
using System.Collections.Generic;
using System.Transactions;
using Nest;
using Transaction = InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm.Transaction;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class PurchaseOrderSearchIndex : BaseEntity
    {
        public PurchaseOrderSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        
        public string PurchaseOrderNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierEmail { get; set; }
        public string CreatedByName { get; set; }
        public string CanceledByName { get; set; }

        public string ConfirmedByName { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal CostFee { get; set; }

        public DateTime DeliveryDate { get; set; }
        public DateTime ConfirmedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public CompletionField Suggest { get; set; }


        public void FillSuggestion()
        {
            List<string> suggestionList = new List<string>();
            suggestionList.Add(PurchaseOrderNumber);
            suggestionList.Add(SupplierName);

            Suggest = new CompletionField
            {
                Input = suggestionList,
                Weight = 1
            };
        }

    }
}