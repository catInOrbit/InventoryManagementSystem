using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteRequestEditRequest : BaseRequest 
    {
        public PriceQuoteOrder PriceQuoteOrder { get; set; }
    }
}