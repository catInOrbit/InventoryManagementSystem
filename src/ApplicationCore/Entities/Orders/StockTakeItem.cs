using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Nest;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class StockTakeItem : BaseEntity
    {
        public StockTakeItem()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string PackageId { get; set; }
        [JsonIgnore] public virtual Package Package { get; set; }
        public int ActualQuantity { get; set; }
        public string Note { get; set; }

        [JsonIgnore] public string StockTakeOrderId { get; set; }
        [JsonIgnore] public virtual StockTakeOrder StockTakeOrder { get; set; }

        [JsonIgnore] [Ignore] [NotMapped] public bool IsShowingPackageDetail { get; set; } = false;

        public bool ShouldSerializePackage()
        {
            if (IsShowingPackageDetail)
                return true;
            return false;
        }

        public bool ShouldSerializeStockTakeOrder()
        {
            return false;
        }

}
}