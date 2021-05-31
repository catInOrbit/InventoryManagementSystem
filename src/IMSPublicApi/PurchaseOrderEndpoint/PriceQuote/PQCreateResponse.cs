using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PQCreateResponse : BaseResponse
    {
        public PriceQuoteOrder PriceQuoteOrder { get; set; }
    }
}