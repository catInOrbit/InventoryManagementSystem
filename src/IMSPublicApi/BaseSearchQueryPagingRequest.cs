namespace InventoryManagementSystem.PublicApi
{
    public abstract class BaseSearchQueryPagingRequest : BaseRequest
    {
        public string Query { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}