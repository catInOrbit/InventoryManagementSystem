using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints
{
    public class STCreateItemResponse : BaseResponse
    {
        public StockTakeOrder StockTakeOrder { get; set; }
    }
    
    public class STIdRequest : BaseRequest
    {
        public string Id { get; set; }
    }
    public class STSearchRequest : BaseRequest
    {
        public string SearchQuery { get; set; }

        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class STSearchResponse : BaseSearchResponse<StockTakeSearchIndex>
    {

    }
    
    public class STIdResponse : BaseSearchResponse<StockTakeOrder>
    {

    }

    public class StockTakeSubmitRequest : BaseRequest
    {
        public string Id { get; set; }
    }
    
    public class STAddRequest : BaseRequest
    {
        public List<string> ProductIds { get; set; }
        public string StockTakeId { get; set; }
    }
    
    public class STSingleUpdateRequest : BaseRequest
    {
        public string StockTakeId { get; set; }
        public string StockTakeItemId { get; set; }
        public int ActualQuantity { get; set; }
        public string Note { get; set; }
    }

    public class STUpdateResponse : BaseResponse
    {
        public List<string> MismatchProductVariantId { get; set; } = new List<string>();
    }
}