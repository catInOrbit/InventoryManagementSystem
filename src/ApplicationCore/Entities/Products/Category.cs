using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Category : BaseEntity
    {
        public override string Id { get; set; }

        public Category()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        
        [JsonIgnore]
        public virtual Transaction Transaction{ get; set; }
    }
}