using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
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
        public virtual bool ShouldSerializeGoodsIssueOrder()
        {
            if(!IsForDisplay)
                return true;
            return false;
        }
        
        
        public virtual bool ShouldSerializePaging()
        {
            if(IsForDisplay)
                return true;
            return false;
        }
        
        public GoodsIssueOrder GoodsIssueOrder { get; set; }
        // public List<GIDisplay> GoodsIssueOrdersDisplays { get; set; } = new List<GIDisplay>();
     
        public PagingOption<GoodsIssueSearchIndex> Paging { get; set; }
  
        [JsonIgnore]
        public bool IsForDisplay { get; set; }
    }
}