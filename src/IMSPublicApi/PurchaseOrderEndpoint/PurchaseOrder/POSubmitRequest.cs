using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class POSubmitRequest : BaseRequest
    {
        public string[] To { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public string PurchaseOrderNumber { get; set; }
        
    }
}