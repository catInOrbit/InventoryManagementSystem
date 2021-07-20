namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.Create
{
    public class SupplierCreateRequest : BaseRequest
    {
        // public Supplier Supplier { get; set; }
        public string SupplierName { get; set; }

        public string Description { get; set; }
        public string Address { get; set; }
        public string SalePersonName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }
    
    public class SupplierUpdateRequest : BaseRequest
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }

        public string Description { get; set; }
        public string Address { get; set; }
        public string SalePersonName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }
    
    public class SupplierResponse : BaseResponse
    {
        public string ModifiedSupplierId { get; set; }
    }
    
    public class SupplierDeleteRequest : BaseRequest
    {
        public string Id { get; set; }
    }
}