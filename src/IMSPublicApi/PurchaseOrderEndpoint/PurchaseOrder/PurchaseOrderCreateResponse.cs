using System;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderCreateResponse : BaseResponse
    {
        public PurchaseOrderCreateResponse(Guid correlationId) : base(correlationId)
        {
            
        }

        public PurchaseOrderCreateResponse()
        {
            
        }

        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
        
    }
}