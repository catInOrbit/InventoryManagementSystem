using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetPriceQuoteResponse : BaseResponse
    {
        public GetPriceQuoteResponse(Guid correlationId) : base(correlationId)
        {
                
        }

        public GetPriceQuoteResponse()
        {
                
        }

        public ICollection<PriceQuoteOrder> PriceQuoteOrders { get; set; }
    }
}