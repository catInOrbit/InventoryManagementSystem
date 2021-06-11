namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProductSearchRequest : BaseRequest
    {
        public string Query { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}