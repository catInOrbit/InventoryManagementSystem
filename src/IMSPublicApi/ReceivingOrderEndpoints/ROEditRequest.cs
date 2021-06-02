namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROEditRequest : BaseRequest
    {
        public string ReceiveOrderGet { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string SupplierInvoice { get; set; }
        public string SupplierId { get; set; }
        public string StorageLocation { get; set; }
    }
}