using System;
using System.Collections;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeOrder : BaseEntity
    {
        public StockTakeOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }
        public virtual ICollection<StockTakeItem> CheckItems { get; set; }
        
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        
        public StockTakeOrderType StockTakeOrderType { get; set; }
    }
}