using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Extensions;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints
{
    public class GIAllRequest : GISearchFilter
    {
        public string SearchQuery { get; set; }
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
        
        public virtual bool ShouldSerializeProductPackageFIFO()
        {
            if(IsSearializingProductPackageFifo)
                return true;
            return false;
        }
        
        public GoodsIssueOrder GoodsIssueOrder { get; set; }
        public List<FifoPackageSuggestion> ProductPackageFIFO { get; set; } =
            new List<FifoPackageSuggestion>();
     
        public PagingOption<GoodsIssueSearchIndex> Paging { get; set; }
  
        [JsonIgnore]
        public bool IsForDisplay { get; set; }

        [JsonIgnore]
        public bool IsSearializingProductPackageFifo { get; set; } = false;

    }
  
    public class GiResponse : BaseResponse
    {
        public GoodsIssueOrder GoodsIssueOrder { get; set; }

        public List<FifoPackageSuggestion> ProductPackageFIFO { get; set; } =
            new List<FifoPackageSuggestion>();

        [JsonIgnore] public bool IsShowingPackageSuggestion { get; set; }
        public bool ShouldSerializeProductPackageFIFO()
        {
            if (IsShowingPackageSuggestion) return true;
            return false;
        }

    }

    public class PackageSuggestion
    {
        public Package Package { get; set; }
        public int NumberOfProductToGet { get; set; }
    }

    public class GoodsReceiptStrategySuggestion
    {
        public string GoodsReceiptId { get; set; }
        public string Location { get; set; }
    }

    public class GiIdRequest
    {
        public string IssueId { get; set; }
    }

    public class GiCreateRequest : BaseRequest
    {
        public string IssueNumber { get; set; }
        
    }
    
    
    public class GiUpdateRequest : BaseRequest
    {
        public string IssueNumber { get; set; }
        public string ChangeStatusTo { get; set; }

    }
    
    public class GiCancel : BaseRequest
    {
        public string IssueNumber { get; set; }
        public string CancelReason { get; set; }

    }
    
    public class GiCancelResponse : BaseResponse
    {
        public GoodsIssueOrder GoodsIssueOrder { get; set; }
    }
}