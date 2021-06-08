using System;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeOrder : BaseEntity
    {
        public StockTakeOrder()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }

        public string ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public string ProductName { get; set; }
        public decimal RecordedQuantity { get; set; }
        public string Note { get; set; }
    }
}