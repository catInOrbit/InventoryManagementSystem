using System;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints
{
    public class GIAllRequest : BaseRequest
    {
        public GISearchFilter GiSearchFilter { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class GISearchRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
        public int Status { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
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