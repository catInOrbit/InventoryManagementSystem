using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Location : BaseEntity
    {
        public Location()
        {
            Id = "LOC" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
        public string LocationName { get; set; }
    }
}