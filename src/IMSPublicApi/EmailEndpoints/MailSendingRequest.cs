namespace InventoryManagementSystem.PublicApi.EmailEndpoints
{
    public class MailSendingRequest : BaseRequest
    {
        public string[] To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

    }
}