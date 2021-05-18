
namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class ResetPasswordRequest : BaseRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
