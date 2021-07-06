using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

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