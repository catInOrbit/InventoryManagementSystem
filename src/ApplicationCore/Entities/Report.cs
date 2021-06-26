using System;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class StockOnhandReport : BaseEntity
    {
        public StockOnhandReport()
        {
            Id = Guid.NewGuid().ToString();
        }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public int StorageQuantity { get; set; }
        public decimal Value { get; set; }
    }
    
    public class StockTakeReport : BaseEntity
    {
        public StockTakeReport()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public DateTime StockTakeDate { get; set; }
        public string ProductName { get; set; }
        public int StorageQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public decimal Value { get; set; }
    }
}