namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROUpdateItemRequest : BaseRequest
    {
        public string ProductVariantId { get; set; }
        public float QuantityUpdate { get; set; }
        public string UnitUpdate { get; set; }
        public string CurrentReceivingOrderId { get; set; }

    }
}