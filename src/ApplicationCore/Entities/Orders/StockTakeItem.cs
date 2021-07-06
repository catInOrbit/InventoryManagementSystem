using System;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeItem : BaseEntity
    {
        public StockTakeItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string ProductVariantId { get; set; }
        [JsonIgnore]
        public virtual ProductVariant ProductVariant { get; set; }
        public int ActualQuantity { get; set; }
        public string Note { get; set; }

        [JsonIgnore]
        public string StockTakeOrderId { get; set; }
        [JsonIgnore]
        public virtual StockTakeOrder StockTakeOrder { get; set; }


    }
}