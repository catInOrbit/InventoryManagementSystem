namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class POCreateRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
    }
}