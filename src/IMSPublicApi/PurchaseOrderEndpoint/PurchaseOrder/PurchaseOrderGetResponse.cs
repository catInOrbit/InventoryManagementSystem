using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderGetResponse : BaseResponse
    {
        public PurchaseOrderGetResponse(Guid correlationId) : base(correlationId)
        {
            
        }

        public PurchaseOrderGetResponse()
        {
            
        }

        public bool Result { get; set; }
        public string Verbose  { get; set; }
        public List<ApplicationCore.Entities.Orders.PurchaseOrder> PurchaseOrders { get; set; }
    }
}