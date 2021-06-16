using System;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class GoodsReceiptOrderItem : BaseEntity
    { 
        public GoodsReceiptOrderItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public string GoodsReceiptOrderId { get; set; }
        [JsonIgnore]
        public virtual GoodsReceiptOrder GoodsReceiptOrder { get; set; }
        public string ProductStorageLocation { get; set; }
        public string ProductVariantId { get; set; }
        public string ProductVariantName { get; set; }
        [JsonIgnore]
        public virtual ProductVariant ProductVariant { get; set; }
        public int QuantityReceived { get; set; }
    }
}