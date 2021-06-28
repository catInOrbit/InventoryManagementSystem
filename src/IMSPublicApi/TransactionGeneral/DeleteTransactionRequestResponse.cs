namespace InventoryManagementSystem.PublicApi.TransactionGeneral
{
    public class DeleteTransactionRequest : BaseRequest
    {
        public string Id { get; set; }
    }
    
    public class DeleteTransactionResponse : BaseResponse
    {
        public bool Status { get; set; }
        public string Verbose { get; set; }
    }
}