using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    [Serializable]
    public class Package : BaseEntity
    {
        public Package()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string ProductVariantId { get; set; }
        [JsonIgnore]
        public virtual ProductVariant  ProductVariant { get; set; }

        public virtual Supplier Supplier { get; set; }
        public string SupplierId { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime ImportedDate { get; set; }
        public string Location { get; set; }

        public string GoodsReceiptOrderId { get; set; }
        [JsonIgnore]
        public virtual GoodsReceiptOrder GoodsReceiptOrder { get; set; }
    }
}