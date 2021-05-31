namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class POConfirmRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
    }
}