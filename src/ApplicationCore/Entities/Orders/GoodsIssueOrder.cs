using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class GoodsIssueOrder : BaseEntity
    {
        public GoodsIssueOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
            GoodsIssueNumber = DateTime.UtcNow.Date.ToString("ddMMyyyy") +
                               Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        }
        
        public string GoodsIssueNumber { get; set; }
        public string RequestId { get; set; }
        
        public string DeliveryMethod { get; set; }
        public string CustomerName { get; set; }
        public string SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
           
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        public GoodsIssueType GoodsIssueType { get; set; }
        
        public virtual ICollection<OrderItem> GoodsIssueProducts { get; set; }
        
        public DateTime DeliveryDate { get; set; }
    }
}
