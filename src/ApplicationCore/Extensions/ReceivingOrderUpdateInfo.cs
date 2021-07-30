namespace InventoryManagementSystem.ApplicationCore.Extensions
{
    public class ReceivingOrderUpdateInfo
    {
        public string ProductVariantId { get; set; }
        public int QuantityReceived { get; set; }
        
        public string Sku { get; set; }
        public string Barcode { get; set; }
    }
}