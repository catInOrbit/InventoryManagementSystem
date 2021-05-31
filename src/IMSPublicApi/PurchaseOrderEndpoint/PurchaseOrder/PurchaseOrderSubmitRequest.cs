using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderSubmitRequest : BaseRequest
    {
        public string[] To { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        
        [JsonIgnore]
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }

    }
}