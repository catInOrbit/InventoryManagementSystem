using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PQEditResponse : BaseResponse
    {
        public PQEditResponse(Guid correlationId) : base(correlationId)
        { }

        public PQEditResponse()
        { }
        
        public PriceQuoteOrder PriceQuoteResponse { get; set; }
        public bool Result { get; set; }
    }
}