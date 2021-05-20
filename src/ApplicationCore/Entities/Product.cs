using Ardalis.GuardClauses;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get;  set; }
        public string PictureUri { get; set; }
        public string CatalogTypeId { get;  set; }
        public string CatalogBrandId { get; set; }
        
        public ICollection<IMSUser> ImsUsers { get; set; }
    }
}