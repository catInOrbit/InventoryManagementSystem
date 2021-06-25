using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints
{
    public class ProductCreateResponse : BaseResponse
    {
        public string CreatedProductId { get; set; }
    }


    public class ProductRequest : BaseRequest
    {
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string Unit { get; set; }
        public string CategoryId { get; set; }
        public List<ProductVairantRequestInfo> ProductVariants { get; set; }
    }

    public class ProductVairantRequestInfo
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }

        public string Sku { get; set; }
        public string Unit { get; set; }
        public int StorageQuantity { get; set; }
        // public string StorageLocation { get; set; }
     
        public List<VariantValueRequestInfo> VariantValues { get; set; }

    }

    public class VariantValueRequestInfo
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
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
    
    public class GetProductAllRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class GetProductSearchRequest
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
        
        public string Category { get; set; }
        public string Strategy { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string FromModifiedDate { get; set; }
        public string ToModifiedDate { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string FromPrice { get; set; }
        public string ToPrice { get; set; }
        public string Brand { get; set; }
    }
    
    public class GetProductSearchResponse : BaseSearchResponse<ProductSearchIndex>
    {
    }
    
}