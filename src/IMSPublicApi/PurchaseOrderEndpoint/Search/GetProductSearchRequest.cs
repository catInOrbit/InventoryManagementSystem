namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetProductSearchRequest : BaseRequest
    {
        public string Query { get; set; }
        
    }
}