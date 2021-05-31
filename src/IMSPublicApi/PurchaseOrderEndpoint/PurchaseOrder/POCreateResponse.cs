using System;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class POCreateResponse : BaseResponse
    {
        public POCreateResponse(Guid correlationId) : base(correlationId)
        {
            
        }

        public POCreateResponse()
        {
            
        }

        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
        
    }
}