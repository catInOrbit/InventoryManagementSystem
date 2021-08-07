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

        public string PkgId { get; set; }
        // [JsonIgnore] public virtual Package Package { get; set; }
        
        public string ProductVariantName { get; set; }
        public string SKU { get; set; }
        public int StorageQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public string Note { get; set; }

        [JsonIgnore] public string StockTakeOrderId { get; set; }
        [JsonIgnore] public virtual StockTakeOrder StockTakeOrder { get; set; }

        [JsonIgnore] [Ignore] [NotMapped] public bool IsShowingPackageId { get; set; } = false;

        public bool ShouldSerializePackageId()
        {
            if (IsShowingPackageId)
                return true;
            return false;
        }

        public bool ShouldSerializeStockTakeOrder()
        {
            return false;
        }

}
}