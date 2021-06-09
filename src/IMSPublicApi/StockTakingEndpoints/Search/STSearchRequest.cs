namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Search
{
    public class STSearchRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
    }
}