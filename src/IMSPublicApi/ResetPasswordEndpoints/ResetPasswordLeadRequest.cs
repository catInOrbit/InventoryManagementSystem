
namespace InventoryManagementSystem.PublicApi.ResetPasswordEndpoints
{
    public class ResetPasswordLeadRequest : BaseRequest
    {
        public string Email { get; set; }
    }
    
    public class ResetPasswordRequest : BaseRequest
    {
        public string Email { get; set; }
    }
}
