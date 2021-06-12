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
        
        public ApplicationCore.Entities.Orders.PurchaseOrder PriceQuoteOrder { get; set; } = new ApplicationCore.Entities.Orders.PurchaseOrder();
        public List<PqDisplay> PriceQuotes { get; set; } = new List<PqDisplay>();

        [JsonIgnore]
        public bool IsForDisplay { get; set; }
        
        
    }
}