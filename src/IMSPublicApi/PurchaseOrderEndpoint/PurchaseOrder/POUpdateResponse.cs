using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class POUpdateResponse : BaseResponse
    {
        public POUpdateResponse(Guid correlationId) : base(correlationId)
        { }

        public POUpdateResponse()
        { }

        public bool Result { get; set; }
        public string Verbose { get; set; }
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }

    }
}