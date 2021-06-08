namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints
{
    public class GiRequest : BaseRequest
    {
        public string IssueNumber { get; set; }
        public string ChangeStatusTo { get; set; }

    }
}