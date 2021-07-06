using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints
{
    public class STCreateItemResponse : BaseResponse
    {
        public bool Status { get; set; }
        public string Verbose { get; set; }
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
        
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string DeliveryMethod { get; set; }
        public string FromDeliveryDate { get; set; }
        public string ToDeliveryDate { get; set; }
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
        public List<StockTakeGroupLocation> StockTakeGroupLocation { get; set; }
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