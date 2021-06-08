using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class GiSearchResponse : BaseResponse
    {
        public GiSearchResponse(Guid correlationId) : base(correlationId)
        {
                
        }

        public GiSearchResponse()
        {
                
        }
        public virtual bool ShouldSerializeGoodsIssueOrders()
        {
            if(!IsForDisplay)
                return true;
            return false;
        }
        
        
        public virtual bool ShouldSerializeGoodsIssueOrdersDisplays()
        {
            if(IsForDisplay)
                return true;
            return false;
        }
        
        public List<GoodsIssueOrder> GoodsIssueOrders { get; set; } = new List<GoodsIssueOrder>();
        public List<GIDisplay> GoodsIssueOrdersDisplays { get; set; } = new List<GIDisplay>();
     
        [JsonIgnore]
        public bool IsForDisplay { get; set; }
    }
}