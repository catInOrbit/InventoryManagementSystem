using System;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class GIDisplay : BaseResponse
    {
        public string Id { get; set; }
        public string GoodsIssueNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CreatedByName { get; set; }
    }
}