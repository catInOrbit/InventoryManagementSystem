using System;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeItem : BaseEntity
    {
        public StockTakeItem()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }
        public string ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        public string ProductName { get; set; }
        public int StorageQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public string Note { get; set; }
    }
}