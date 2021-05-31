using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteResponse : BaseResponse
    {
        public PriceQuoteOrder PriceQuoteOrder { get; set; }
    }
}