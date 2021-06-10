﻿namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROUpdateItemRequest : BaseRequest
    {
        public string ProductVariantId { get; set; }
        public int QuantityUpdate { get; set; }
        public string UnitUpdate { get; set; }
        public string CurrentReceivingOrderNumber { get; set; }

    }
}