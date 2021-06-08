namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints
{
    public class STUpdateRequest : BaseRequest
    {
        public string StockTakeId { get; set; }
        public int RecordedQuantity { get; set; }
        public string Note { get; set; }
        public string ProductVariantId { get; set; }
    }
}