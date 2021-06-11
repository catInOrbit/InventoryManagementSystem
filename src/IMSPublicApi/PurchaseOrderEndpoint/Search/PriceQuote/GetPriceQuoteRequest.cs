namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote
{
    public class GetPriceQuoteRequest : BaseRequest
    {
        public string Number { get; set; }
        
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}