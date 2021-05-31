namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetProductRequest : BaseRequest
    {
        public string ProductId { get; set; }
    }
}