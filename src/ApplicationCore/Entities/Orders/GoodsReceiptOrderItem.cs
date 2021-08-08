using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
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
        public string ProductVariantId { get; set; }
        public string ProductVariantName { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        
        [NotMapped]
        public string VariantSku { get; set; }
        [NotMapped]
        public string VariantBarcode { get; set; }
        public int QuantityReceived { get; set; }
        
        [NotMapped]
        [JsonIgnore]
        public bool IsShowingProductVariant { get; set; }

        public bool ShouldSerializeGoodsReceiptOrder()
        {
            return false;
        }

        public bool ShouldSerializeProductVariant()
        {
            return false;
        }
        
        [OnSerializing]
        public void FormatProductVariantResponse(StreamingContext context)
        {
            ProductVariant.IsShowingTransaction = false;
        }
        
    }
}