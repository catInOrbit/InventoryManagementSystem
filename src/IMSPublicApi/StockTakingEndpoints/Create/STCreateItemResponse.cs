using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Create
{
    public class STCreateItemResponse : BaseResponse
    {
        public StockTakeOrder StockTakeOrder { get; set; }
    }
}