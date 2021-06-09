using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Create
{
    public class STCreateResponse : BaseResponse
    {
        public StockTakeOrder StockTakeOrder { get; set; }
    }
}