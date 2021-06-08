using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Create
{
    public class GiResponse : BaseResponse
    {
        public GoodsIssueOrder GoodsIssueOrder { get; set; }
                
    }
}