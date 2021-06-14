using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.Create
{
    public class SupplierCreateRequest : BaseRequest
    {
        public Supplier Supplier { get; set; }
    }
}