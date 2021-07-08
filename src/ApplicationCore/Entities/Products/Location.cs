using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Location : BaseEntity
    {
        public Location()
        {
            Id =  Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            LocationBarcode = "LC" + Id;
        }
        
        public string LocationBarcode { get; set; }
        public string LocationName { get; set; }

        [JsonIgnore]
        public Transaction Transaction { get; set; }
    }
}