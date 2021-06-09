using System;
using System.Collections;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeOrder : BaseEntity
    {
        public StockTakeOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }

        public string ProductVariantId { get; set; }
        public ICollection<OrderItem> Products { get; set; }
        public string ProductName { get; set; }
        public int RecordedQuantity { get; set; }
        public string Note { get; set; }
        
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}