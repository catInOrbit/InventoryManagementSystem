using System;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint
{
    public class PurchaseRequestResponse : BaseResponse
    {
        public PurchaseRequestResponse(Guid correlationId) : base(correlationId)
        {
            
        }

        public PurchaseRequestResponse()
        { }

        public bool Result { get; set; }
    }
}