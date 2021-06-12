using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PQCreateResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrderPQ { get; set; }
    }
}