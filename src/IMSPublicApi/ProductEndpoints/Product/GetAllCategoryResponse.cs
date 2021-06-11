using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetAllCategoryResponse : BaseResponse
    {
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}