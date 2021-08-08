namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class AuthenticateRequest : BaseRequest 
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
