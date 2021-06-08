using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class GIDisplay
    {
        public string Id { get; set; }
        public string GoodsIssueNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CreatedByName { get; set; }
    }
}