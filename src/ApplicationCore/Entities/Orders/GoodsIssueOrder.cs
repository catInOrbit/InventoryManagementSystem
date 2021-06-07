using System;
using System.Collections.Generic;
using System.Transactions;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class GoodsIssueOrder : BaseEntity
    {
        public GoodsIssueOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }
        
        public string GoodsIssueNumber { get; set; }
        public string PurchaseOrderId { get; set; }
        
        public string SupplierId { get; set; }
        public Supplier Supplier { get; set; }
           
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        
        public GoodsIssueType GoodsIssueType { get; set; }

        
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}