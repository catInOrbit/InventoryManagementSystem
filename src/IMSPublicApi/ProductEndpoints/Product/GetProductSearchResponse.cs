using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProductSearchResponse
    {
        public PagingOption<ProductSearchIndex> Paging { get; set; }
    }
}