namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Search
{
    public class STSearchRequest : BaseRequest
    {
        public string SearchQuery { get; set; }

        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}