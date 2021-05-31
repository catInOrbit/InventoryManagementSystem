namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetPurchaseOrderRequest : BaseRequest
    {
        public string number { get; set; }
    }
}