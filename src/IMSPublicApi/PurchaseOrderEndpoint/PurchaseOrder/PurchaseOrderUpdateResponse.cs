using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderUpdateResponse : BaseResponse
    {
        public PurchaseOrderUpdateResponse(Guid correlationId) : base(correlationId)
        { }

        public PurchaseOrderUpdateResponse()
        { }

        public bool Result { get; set; }
        public string Verbose { get; set; }
    }
}