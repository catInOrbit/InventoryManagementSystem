using System;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeItem : BaseEntity
    {
        public StockTakeItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string PackageId { get; set; }
        public virtual Package Package { get; set; }
        public int ActualQuantity { get; set; }
        public string Note { get; set; }

        [JsonIgnore]
        public string StockTakeOrderId { get; set; }
        [JsonIgnore]
        public virtual StockTakeOrder StockTakeOrder { get; set; }


    }
}