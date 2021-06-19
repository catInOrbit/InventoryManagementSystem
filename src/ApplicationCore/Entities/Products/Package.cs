using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class Package : BaseEntity
    {
        public Package()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string ProductVariantId { get; set; }
        public virtual ProductVariant  ProductVariant { get; set; }
        public decimal TotalImportPrice { get; set; }
        public int TotalImportQuantity { get; set; }
        public DateTime ImportedDate { get; set; }
        public string Location { get; set; }

        public string GoodsReceiptOrderId { get; set; }
        [JsonIgnore]
        public virtual GoodsReceiptOrder GoodsReceiptOrder { get; set; }
    }
}