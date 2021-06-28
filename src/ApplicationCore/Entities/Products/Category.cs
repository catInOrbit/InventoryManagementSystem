using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }
}