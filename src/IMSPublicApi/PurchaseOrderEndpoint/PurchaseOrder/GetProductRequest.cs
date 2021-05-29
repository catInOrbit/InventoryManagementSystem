namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class GetProductRequest : BaseRequest
    {
        public string ProductId { get; set; }
    }
}