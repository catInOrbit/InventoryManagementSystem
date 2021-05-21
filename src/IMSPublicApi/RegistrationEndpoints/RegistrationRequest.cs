namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class RegistrationRequest : BaseRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        
        public string FullName { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Address { get; set; }


    }
}