using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class GoodsIssueSearchIndex : BaseSearchIndex
    {
        public override string Id { get; set; }

        public GoodsIssueSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        
        public string  TransactionId { get; set; }
        public string  GoodsIssueNumber { get; set; }
        public string  GoodsIssueRequestNumber { get; set; }
        
        public string  DeliveryMethod { get; set; }
        public string  DeliveryAddress { get; set; }

        public string  Status { get; set; }
        public string  CreatedByName { get; set; }
        public DateTime  DeliveryDate { get; set; }
        public DateTime  CreatedDate { get; set; }
        
        public DateTime  ModifyDate { get; set; }


    }
}