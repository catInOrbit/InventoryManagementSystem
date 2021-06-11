namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
    public class ROGetRequest : BaseRequest
    {
        public string Query { get; set; }
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}