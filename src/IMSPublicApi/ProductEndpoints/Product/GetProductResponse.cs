
namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProductResponse : BaseResponse
    {
        public ApplicationCore.Entities.Products.Product Product { get; set; }
    }
}