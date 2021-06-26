using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.Create
{
    public class SupplierCreateRequest : BaseRequest
    {
        public Supplier Supplier { get; set; }
    }
    
    public class SupplierUpdateRequest : BaseRequest
    {
        public string SupplierId { get; set; }
        
        public Supplier Supplier { get; set; }
    }
    
    public class SupplierDeleteRequest : BaseRequest
    {
        public string Id { get; set; }
    }
}