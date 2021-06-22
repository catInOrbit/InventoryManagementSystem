using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseRequisition.Update
{
    public class RUpdateRequest
    {
        public DateTime Deadline { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}