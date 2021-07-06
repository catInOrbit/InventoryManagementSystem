using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeGroupLocation : BaseEntity
    {

        public StockTakeGroupLocation()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public string LocationId { get; set; }
        
        [JsonIgnore]
        public virtual Location Location { get; set; }
        public virtual ICollection<StockTakeItem> CheckItems { get; set; }
    }
}