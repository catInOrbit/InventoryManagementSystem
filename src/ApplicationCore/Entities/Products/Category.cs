using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Nest;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Category : BaseSearchIndex
    {
        public override string Id { get; set; }

        public Category()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        
        public string TransactionId { get; set; }
        [JsonIgnore]
        [Ignore]
        public virtual Transaction Transaction{ get; set; }
    }
}