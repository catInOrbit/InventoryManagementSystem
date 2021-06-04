namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
    public class ROGetRequest : BaseRequest
    {
        public string OrderNumber { get; set; }
    }
}