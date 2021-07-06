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
            Id = "ST"+Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public virtual ICollection<StockTakeGroupLocation> GroupLocations { get; set; } =
            new List<StockTakeGroupLocation>();
        
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        
        public StockTakeOrderType StockTakeOrderType { get; set; }
    }
}