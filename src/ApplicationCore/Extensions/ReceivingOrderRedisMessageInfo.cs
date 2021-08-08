namespace InventoryManagementSystem.ApplicationCore.Extensions
{
    public class ReceivingOrderProductUpdateInfo
    {
        public string ProductVariantId { get; set; }
        public int QuantityReceived { get; set; }
        public string Sku { get; set; }
    }
    
    public class ExistRedisVariantSKU
    {
        public string ProductVariantId { get; set; }
        public string RedisSKU { get; set; }
    }
}