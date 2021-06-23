namespace InventoryManagementSystem.PublicApi.TransactionGeneral
{
    public class DeleteTransactionRequest : BaseRequest
    {
        public string Id { get; set; }
        public int Type { get; set; }
    }
}