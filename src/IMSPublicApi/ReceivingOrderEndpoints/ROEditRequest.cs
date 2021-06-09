namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROEditRequest : BaseRequest
    {
        public string ReceiveOrderId { get; set; }
        public string PurchaseOrderId { get; set; }
        public string StorageLocation { get; set; }
    }
}