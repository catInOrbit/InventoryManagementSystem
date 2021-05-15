namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class RegisterRequest : BaseRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
}