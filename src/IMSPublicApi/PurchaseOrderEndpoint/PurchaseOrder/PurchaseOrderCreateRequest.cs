using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderCreateRequest : BaseRequest
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
}