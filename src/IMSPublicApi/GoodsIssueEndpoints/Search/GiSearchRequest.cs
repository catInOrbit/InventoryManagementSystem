namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class GiSearchRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
    }
}