namespace InventoryManagementSystem.PublicApi.LoggingOutEndpoints
{
    public class LoggingOutResponse : BaseResponse
    {
        public bool Result { get; set; } = false;
        public string Verbose { get; set; }
    }
}