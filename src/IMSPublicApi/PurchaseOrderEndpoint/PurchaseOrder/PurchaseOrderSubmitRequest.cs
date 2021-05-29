namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderSubmitRequest : BaseRequest
    {
        public string[] SupplierEmail { get; set; }
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }

    }
}