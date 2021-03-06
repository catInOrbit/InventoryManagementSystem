using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Extensions;

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
    
    public class ROSKUExistanceRequest : BaseRequest
    {
        public string ReceiptPurchaseOrderId { get; set; }
    }
    
    public class ROSKUExistanceResponse : BaseResponse
    {
        public List<ExistRedisVariantSKU> ExistRedisVariantSkus { get; set; }
    }
    
    public class ROCancelRequest : BaseRequest
    {
        public string ReceivingOrderId { get; set; }
        public string CancelReason { get; set; }

    }
    
    public class ROSubmitResponse : BaseRequest
    {
        public string IncompletePurchaseOrderId { get; set; }
        public List<string> IncompleteVariantId { get; set; } = new List<string>();
    }
         
    public class ROUpdateRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
        public string LocationId { get; set; }
        public List<ReceivingOrderProductUpdateInfo> UpdateItems { get; set; }
    }
    
    public class ROUpdateResponse : BaseRequest
    {
        public string TransactionId { get; set; }
        public string CreatedGoodsReceiptId { get; set; }
    }

 
    
    public class ROSingleProductUpdateRequest : BaseRequest
    {
        public string ProductVariantId { get; set; }
        public string Sku { get; set; }
    }

}