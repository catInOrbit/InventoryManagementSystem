using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Nest;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    [Serializable]
    public class Package : BaseSearchIndex
    {
        public Package()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string ProductVariantId { get; set; }
        // [Ignore]
        public virtual ProductVariant  ProductVariant { get; set; }

        [Ignore]
        public virtual Supplier Supplier { get; set; }
        public string SupplierId { get; set; }

        [Column(TypeName = "decimal(16,3)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime ImportedDate { get; set; }
        
        public string LocationId { get; set; }
        public virtual Location Location { get; set; }
        
        public string TransactionId { get; set; }
        [Ignore]
        public virtual Transaction Transaction { get; set; }
        public string GoodsReceiptOrderId { get; set; }
        [Ignore]
        public virtual GoodsReceiptOrder GoodsReceiptOrder { get; set; }
        
        [JsonIgnore]
        [Ignore]
        [NotMapped]
        public bool IsShowingTransaction { get; set; }
        
        [JsonIgnore]
        [Ignore]
        [NotMapped]
        public bool IsShowingProductVariant { get; set; }

        [JsonIgnore]
        [Ignore]
        [NotMapped]
        public bool IsDisplayingDetail { get; set; }

        public bool ShouldSerializeProductVariant()
        {
            if (IsShowingProductVariant)
                return true;
            return false;
        }
        
        public bool ShouldSerializeGoodsReceiptOrder()
        {
            if (IsDisplayingDetail)
                return true;
            return false;
        }
        
        public bool ShouldSerializeSupplier()
        {
            if (IsDisplayingDetail)
                return true;
            return false;
        }
        

        public bool ShouldSerializeTransaction()
        {
            if (IsShowingTransaction)
                return true;
            return false;
        }
        
        [OnSerializing]
        public void FormatProductVariantResponse(StreamingContext context)
        {
            if(ProductVariant != null)
                ProductVariant.IsShowingTransaction = false;
        }
    }
}