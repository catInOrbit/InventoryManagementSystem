namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROEditRequest : BaseRequest
    {
        public string ReceiveOrderNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string StorageLocation { get; set; }
    }
}