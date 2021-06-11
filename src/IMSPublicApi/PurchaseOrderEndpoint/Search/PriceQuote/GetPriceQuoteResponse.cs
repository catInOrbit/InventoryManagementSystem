using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Newtonsoft.Json;

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
        public virtual bool ShouldSerializePriceQuoteOrders()
        {
            if(!IsForDisplay)
                return true;
            return false;
        }
        
        
        public virtual bool ShouldSerializePriceQuoteOs()
        {
            if(IsForDisplay)
                return true;
            return false;
        }
        
        public PriceQuoteOrder PriceQuoteOrder { get; set; } = new PriceQuoteOrder();
        public List<PQDisplay> PriceQuotes { get; set; } = new List<PQDisplay>();

        [JsonIgnore]
        public bool IsForDisplay { get; set; }
        
        
    }
}