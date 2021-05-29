using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class GetProductResponse : BaseResponse
    {
        public Product Product { get; set; }
    }
}