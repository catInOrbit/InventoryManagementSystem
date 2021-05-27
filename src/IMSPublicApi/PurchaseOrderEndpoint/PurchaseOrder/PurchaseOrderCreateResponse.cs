using System;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderCreateResponse : BaseResponse
    {
        public PurchaseOrderCreateResponse(Guid correlationId) : base(correlationId)
        { }

        public PurchaseOrderCreateResponse()
        { }

        public bool Result { get; set; }
        public string Verbose { get; set; }
    }
}