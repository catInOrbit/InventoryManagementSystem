﻿using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class GoodsReceiptOrderSearchIndex : BaseSearchIndex
    {
        public override string Id { get; set; }

        public GoodsReceiptOrderSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        
        public string  TransactionId { get; set; }

        public string PurchaseOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string SupplierName { get; set; }
        public string CreatedBy { get; set; }
    }
}