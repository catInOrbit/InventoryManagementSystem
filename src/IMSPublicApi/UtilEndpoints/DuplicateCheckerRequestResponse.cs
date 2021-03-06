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
        public bool HasMatch { get; set; } = false;
        public List<BaseSearchIndex> DatabaseMatchList { get; set; } = new List<BaseSearchIndex>();
        
    }
    
    public class DuplicateCheckerUserResponse : BaseResponse
    {
        public bool HasMatch { get; set; } = false;
        public ApplicationUser MatchedUser  { get; set; }       
    }
    
    public class DuplicateProductVariantCheckerResponse : BaseResponse
    {
        public bool HasMatch { get; set; } = false;
        public List<BaseSearchIndex> DatabaseMatchList { get; set; } = new List<BaseSearchIndex>();
        public List<ProductUpdateMessage> RedisMatchList { get; set; } = new List<ProductUpdateMessage>();

    }
}