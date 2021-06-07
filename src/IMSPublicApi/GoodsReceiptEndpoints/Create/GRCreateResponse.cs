using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.GoodsReceiptEndpoints.Create
{
    public class GRCreateResponse : BaseResponse
    {
        public GoodsIssueOrder GoodsIssueOrder { get; set; }
    }
}