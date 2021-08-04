using System.Collections;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.RedisMessages;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.UtilEndpoints
{
    public class DuplicateCheckerRequest : BaseRequest
    {
        public string Value { get; set; }
    }
    
    public class DuplicateCheckerResponse : BaseResponse
    {
        public bool HasMatch { get; set; }
        public List<BaseSearchIndex> DatabaseMatchList { get; set; } = new List<BaseSearchIndex>();
        
    }
    
    public class DuplicateProductVariantCheckerResponse : BaseResponse
    {
        public bool HasMatch { get; set; }
        public List<BaseSearchIndex> DatabaseMatchList { get; set; } = new List<BaseSearchIndex>();
        public List<ProductUpdateMessage> RedisMatchList { get; set; } = new List<ProductUpdateMessage>();

    }
}