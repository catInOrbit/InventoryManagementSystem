namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class POResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
}