using Ardalis.ApiEndpoints;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrderProduct
{
    public class PurchaseOrderProductRequest : BaseRequest
    {
        public string PurchaseOrderId { get; set; }
        public string SearchedProductId { get; set; }
    }
}