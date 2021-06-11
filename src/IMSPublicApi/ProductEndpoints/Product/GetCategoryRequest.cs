namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetCategoryRequest : BaseRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}