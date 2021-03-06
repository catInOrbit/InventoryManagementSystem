using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class GoodsIssueOrder : BaseEntity
    {
        public GoodsIssueOrder()
        {
            Id = "GO"+Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
        public string RequestId { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryAddress { get; set; }
        public string SupplierId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        public GoodsIssueStatusType GoodsIssueType { get; set; }
        public virtual ICollection<OrderItem> GoodsIssueProducts { get; set; }
        [NotMapped]
        public string GoodsIssueOrderStatusString { get; set; }
        public DateTime DeliveryDate { get; set; }

 
        
        [OnSerializing]
        public void FormatResponse(StreamingContext context)
        {
            GoodsIssueOrderStatusString = GoodsIssueType.ToString();
            foreach (var goodsIssueProduct in GoodsIssueProducts)
                goodsIssueProduct.IsShowingProductVariant = true;
        }
    }
}
