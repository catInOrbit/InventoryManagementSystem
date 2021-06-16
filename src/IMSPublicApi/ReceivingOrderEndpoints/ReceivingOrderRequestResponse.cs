using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ROEditRequest : BaseRequest
    {
        public string ReceiveOrderNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
    }
    
    public class ROCreateResponse : BaseResponse
    {
        public ROCreateResponse(Guid correlationId) : base(correlationId)
        { }
    
        public ROCreateResponse()
        { }
    
        public GoodsReceiptOrder ReceivingOrder { get; set; }
    }
        
    public class ROSubmitRequest : BaseRequest
    {
        public string ReceivingOrderId { get; set; }
    }
         
    public class ROAdjustRequest : BaseRequest
    {
        public string CurrentReceivingOrderNumber { get; set; }
        public List<GoodsReceiptOrderItem> UpdateItems { get; set; }
    }
}