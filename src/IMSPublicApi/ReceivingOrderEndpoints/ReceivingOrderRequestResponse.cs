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
    
    public class ROSubmitResponse : BaseRequest
    {
        public string IncompletePurchaseOrderId { get; set; }
        public List<string> IncompleteVariantId { get; set; } = new List<string>();
    }
         
    public class ROUpdateRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
        public List<ROItemUpdateRequest> UpdateItems { get; set; }
    }
    
    public class ROUpdateResponse : BaseRequest
    {
        public string CreatedGoodsReceiptId { get; set; }
    }

    public class ROItemUpdateRequest
    {
        public string ProductVariantId { get; set; }
        public int QuantityReceived { get; set; }
    }
    
    public class ROSingleProductUpdateRequest : BaseRequest
    {
        public string ProductVariantId { get; set; }
        public string Sku { get; set; }
        public decimal SalePrice { get; set; }
    }
}