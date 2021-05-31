using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetProductResponse : BaseResponse
    {
        public Product Product { get; set; }
    }
}