namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseRequisition.Create
{
    public class RCreateResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; } =
            new ApplicationCore.Entities.Orders.PurchaseOrder();
    }
}