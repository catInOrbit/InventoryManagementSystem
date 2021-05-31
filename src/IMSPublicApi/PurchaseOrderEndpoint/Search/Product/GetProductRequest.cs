namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.Product
{
    public class GetProductRequest : BaseRequest
    {
        public string ProductId { get; set; }
    }
}