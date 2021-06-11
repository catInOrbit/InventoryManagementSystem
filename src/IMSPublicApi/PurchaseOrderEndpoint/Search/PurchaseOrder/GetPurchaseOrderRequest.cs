namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetAllPurchaseOrderRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}