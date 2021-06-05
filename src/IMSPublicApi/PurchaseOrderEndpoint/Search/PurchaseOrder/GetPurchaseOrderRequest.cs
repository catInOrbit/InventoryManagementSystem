namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetAllPurchaseOrderRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
    }
}