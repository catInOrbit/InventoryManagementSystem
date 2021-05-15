namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class AuthenticateRequest : BaseRequest 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
