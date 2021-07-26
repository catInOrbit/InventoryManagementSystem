using System.Collections;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
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
        public List<BaseSearchIndex> MatchList { get; set; } = new List<BaseSearchIndex>();
    }
}