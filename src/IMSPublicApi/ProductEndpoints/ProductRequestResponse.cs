using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints
{
    public class ProductCreateRequest : BaseRequest
    {
        public ApplicationCore.Entities.Products.Product Product { get; set; }
    }
    
    public class ProductUpdateRequest : BaseRequest
    {
        public string Id { get; set; }
        public ApplicationCore.Entities.Products.Product ProductUpdate { get; set; }
        
    }
    
    public class GetAllCategoryResponse : BaseResponse
    {
        public List<Category> Categories { get; set; } = new List<Category>();
    }
    
    public class GetCategoryRequest : BaseRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class GetProductRequest : BaseRequest
    {
        public string ProductId { get; set; }
    }
    
    public class GetProductResponse : BaseResponse
    {
        public ApplicationCore.Entities.Products.Product Product { get; set; }
    }
    
    public class GetProductSearchRequest : BaseSearchQueryPagingRequest
    {
  
    }
    
    public class GetProductSearchResponse : BaseSearchResponse<ProductSearchIndex>
    {
    }
    
}