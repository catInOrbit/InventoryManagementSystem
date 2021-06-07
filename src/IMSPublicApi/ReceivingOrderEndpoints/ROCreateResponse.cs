using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROCreateResponse : BaseResponse
    {
        public ROCreateResponse(Guid correlationId) : base(correlationId)
        { }

        public ROCreateResponse()
        { }

        public GoodsReceiptOrder ReceivingOrder { get; set; }
    }
}