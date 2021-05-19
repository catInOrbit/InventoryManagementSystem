namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class RegistrationRequest : BaseRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public string RoleName { get; set; }

    }
}