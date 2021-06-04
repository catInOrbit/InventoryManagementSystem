namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROSubmitRequest : BaseRequest
    {
        public string ReceivingOrderId { get; set; }
    }
}