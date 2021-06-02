using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class ReceivedOrderItem : BaseEntity
    {
        public string ReceivedOrderId { get; set; }
        public virtual ReceivingOrder ReceivingOrder { get; set; }

        public string StorageLocation { get; set; }
        public string ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        public float Quantity { get; set; }
        public float QuantityReceived { get; set; }
        public float QuantityInventory { get; set; }
    }
}