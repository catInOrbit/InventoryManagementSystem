using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote
{
    public class GetPriceQuoteResponse : BaseResponse
    {
        public GetPriceQuoteResponse(Guid correlationId) : base(correlationId)
        {
                
        }

        public GetPriceQuoteResponse()
        {
                
        }

        public List<PriceQuoteOrder> PriceQuoteOrders { get; set; } = new List<PriceQuoteOrder>();
    }
}