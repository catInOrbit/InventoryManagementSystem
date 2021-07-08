using InventoryManagementSystem.ApplicationCore.Entities.Products;

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
        public Location Location { get; set; }
    }
}