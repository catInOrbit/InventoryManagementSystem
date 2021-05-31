
namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.Product
{
    public class GetProductResponse : BaseResponse
    {
        public ApplicationCore.Entities.Products.Product Product { get; set; }
    }
}