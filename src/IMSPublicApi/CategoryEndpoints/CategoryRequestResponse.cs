using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.PublicApi.CategoryEndpoints
{
    public class CategoryCreateRequest
    {
        public Category Category { get; set; }
    }
    
    public class CategoryUpdateRequest
    {
        public string CategoryId { get; set; }
        public Category CategoryUpdateInfo { get; set; }
    }
    
    public class CategorySearchIdRequest
    {
        public string Id { get; set; }
    }
    
    public class CategorySearchIdResponse
    {
        public Category Category { get; set; }
    }
    
    public class CategoryUpdateResponse
    {
        public bool Status { get; set; }
        public string Verbose { get; set; }
    }
}