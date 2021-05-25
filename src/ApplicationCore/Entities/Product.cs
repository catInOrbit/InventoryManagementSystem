using Ardalis.GuardClauses;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class Product : BaseEntity
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string Sku { get; set; }
        public string BrandId { get; set; }
        public string CategoryId { get; set; }
        public float ProductPrice { get; set; }
        public string Unit { get; set; }
        
        public ICollection<UserInfo> ImsUsers { get; set; }
        public ICollection<Category> Category { get; set; }
        public ICollection<Brand> Brand { get; set; }

    }
}