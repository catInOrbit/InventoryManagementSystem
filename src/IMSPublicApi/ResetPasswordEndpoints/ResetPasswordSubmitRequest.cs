namespace InventoryManagementSystem.PublicApi.ResetPasswordEndpoints
{
    public class ResetPasswordSubmitRequest : BaseRequest
    {
        public string Token { get; set; }
        public string Email { get; set; }
        
        public string NewPassword { get; set; }

    }
}