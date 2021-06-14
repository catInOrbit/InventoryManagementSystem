namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.SupplierSearch
{
    public class GetAllSupplierRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}