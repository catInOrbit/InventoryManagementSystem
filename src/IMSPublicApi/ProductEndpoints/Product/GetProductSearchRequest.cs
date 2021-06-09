namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProductSearchRequest : BaseRequest
    {
        public string Query { get; set; }
        
    }
}