namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class GiSearchRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}