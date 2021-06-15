using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
    public class ROGetRequest : BaseRequest
    {
        public string Query { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
      public class ROGetResponse : BaseResponse
        {
            public ROGetResponse(Guid correlationId) : base()
            {
                base._correlationId = correlationId;
            }
            
            [JsonIgnore]
            public bool IsDislayingAll { get; set; }
    
            public bool ShouldSerializePaging()
            {
                if (IsDislayingAll) 
                    return true;
                return false;
            }
    
            public ROGetResponse()
            { }
    
            public GoodsReceiptOrder ReceiveingOrder { get; set; }
    
            // public List<GoodsReceiptOrderSearchIndex> ReceiveingOrderSearchIndex { get; set; } = new List<GoodsReceiptOrderSearchIndex>();
            public PagingOption<GoodsReceiptOrderSearchIndex> Paging { get; set; }
    
        }
        
        public class ROIdGetRequest : BaseRequest
        {
            public string Id { get; set; }
        }

        public class POsForRORequest : BaseRequest
        {
            public string Id { get; set; }
        }
        public class POsForROResponse : BaseResponse
        {
            public List<string> PurchaseOrderIdList { get; set; } = new List<string>();
        }

}