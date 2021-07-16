using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints
{
    public class LocationCreateRequest : BaseRequest
    {
        public string LocationName { get; set; }
        public string LocationBarcode { get; set; }
    }
    
    public class LocationUpdateRequest : BaseRequest
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationBarcode { get; set; }
    }
    
    public class LocationResponse : BaseRequest
    {
        public ApplicationCore.Entities.Products.Location Location { get; set; }
    }
    
    public class LocationSearchRequest : LocationSearchFilter
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class LocationSearchResponse : BaseSearchResponse<ApplicationCore.Entities.Products.Location>
    {
    }
}