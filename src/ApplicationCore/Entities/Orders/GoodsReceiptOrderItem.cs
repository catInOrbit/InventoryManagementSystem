﻿using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class GoodsReceiptOrderItem : BaseEntity
    {
        public string ReceivedOrderId { get; set; }
        public virtual GoodsReceiptOrder ReceivingOrder { get; set; }
        public string StorageLocation { get; set; }
        public string ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        public int Quantity { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityInventory { get; set; }
    }
}