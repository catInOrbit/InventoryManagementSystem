namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.Product
{
    public class GetProductSearchRequest : BaseRequest
    {
        public string Query { get; set; }
        
    }
}