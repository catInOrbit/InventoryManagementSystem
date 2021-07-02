using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.PublicApi.CategoryEndpoints
{
    public class CategoryCreateRequest
    {
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }
    
    public class CategoryUpdateRequest
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
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