using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class POUpdateRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
        public string SupplierId { get; set; }
        public ICollection<OrderItem> OrderItemInfos  { get; set; }
    }
}