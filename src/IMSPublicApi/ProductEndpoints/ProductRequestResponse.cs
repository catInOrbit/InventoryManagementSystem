using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.RedisMessages;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints
{
    public class ProductCreateResponse : BaseResponse
    {
        public string CreatedProductId { get; set; }
    }
    
    
    public class ProductImageRequest : BaseRequest
    {
        public string Id { get; set; }
        public string ImageLink { get; set; }
    }

    public class ProductUpdateRequest : BaseRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        public string CategoryId { get; set; }
        public string ProductImageLink { get; set; }
        public string Unit { get; set; }

    }
    
    public class ProductVariantUpdateRequest : BaseRequest
    {
        public string ProductId { get; set; }
        public bool IsVariantType { get; set; }
        public List<ProductVairantUpdateRequestInfo> ProductVariantsUpdate { get; set; }
    }

    public class ProductUpdateResponse : BaseResponse
    {
        public ApplicationCore.Entities.Products.Product Product { get; set; }
    }

    

    public class ProductRequest : BaseRequest
    {
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        public string Unit { get; set; }
        public string CategoryId { get; set; }
        public string ProductImageLink { get; set; }
        public bool IsVariantType { get; set; }

        public List<ProductVairantRequestInfo> ProductVariants { get; set; }
    }

    public class ProductVairantRequestInfo
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }

        public string Barcode { get; set; }

        public string Sku { get; set; }
        // public string StorageLocation { get; set; }
     
        // public List<VariantValueRequestInfo> VariantValues { get; set; }

    }
    
    public class ProductVairantUpdateRequestInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public string ProductVariantImageLink { get; set; }
        public string Sku { get; set; }
        // public string StorageLocation { get; set; }
     
        // public List<VariantValueRequestInfo> VariantValues { get; set; }

    }

    public class VariantValueRequestInfo
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
    
    public class GetAllCategoryResponse : BaseSearchResponse<Category>
    {
        // public List<Category> Categories { get; set; } = new List<Category>();
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
    
    public class GetProductVariantRequest : BaseRequest
    {
        public string ProductVariantId { get; set; }
    }
    
    public class GetProductResponse : BaseResponse
    {
        public ApplicationCore.Entities.Products.Product Product { get; set; }
        public ProductVariant ProductVariant { get; set; }
        
        [JsonIgnore]
        public bool IsGettingVariant { get; set; }

        public bool ShouldSerializeProductVariant()
        {
            if (IsGettingVariant)
                return true;
            return false;
        }
        
        public bool ShouldSerializeProduct()
        {
            if (IsGettingVariant)
                return false;
            return true;
        }
    }
    
    public class GetProductAllRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class GetProductVariantSearchRequest : ProductVariantSearchFilter
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class GetProductSearchRequest : ProductSearchFilter
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class GetBrandRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
        
    }
    
    
    
    public class GetProductVariantSearchResponse : BaseSearchResponse<ProductVariantSearchIndex>
    {
    }
    
    public class GetProductSearchResponse : BaseSearchResponse<ProductSearchIndex>
    {
    }
    
    public class GetBrandResponse : BaseSearchResponse<Brand>
    {
    }

    public class ProductUpdateMessageResponse
    {
        public List<ProductUpdateMessage> ProductUpdateMessages { get; set; }
    }
    
}