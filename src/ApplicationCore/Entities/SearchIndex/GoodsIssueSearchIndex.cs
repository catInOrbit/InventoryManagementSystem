using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class GoodsIssueSearchIndex : BaseEntity
    {
        public GoodsIssueSearchIndex()
        {
            Id = Guid.NewGuid().ToString() + "-ignore-id";
        }
        
        public string  GoodsIssueNumber { get; set; }
        public string  GoodsIssueRequestNumber { get; set; }
        public string  Status { get; set; }
        public string  CreatedByName { get; set; }
        public DateTime  DeliveryDate { get; set; }
    }
}