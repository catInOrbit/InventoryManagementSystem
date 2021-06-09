namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProductRequest : BaseRequest
    {
        public string ProductId { get; set; }
    }
}