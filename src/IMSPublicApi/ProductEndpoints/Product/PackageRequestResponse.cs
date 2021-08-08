using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class PackageRequest
    {
        public string PackageId { get; set; }
    }
    
    public class PackageSearchRequest : PackageSearchFilter
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
        
        public bool IsLocationOnly { get; set; }
    }
    
    public class PackageResponse
    {
        public Package Package { get; set; }
    }
    
    public class PackageSearchResponse : BaseSearchResponse<Package>
    {
    }
    
    public class LocationSearchResponse : BaseSearchResponse<ApplicationCore.Entities.Products.Location>
    {
    }
}