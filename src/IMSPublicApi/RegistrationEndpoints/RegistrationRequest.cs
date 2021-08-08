using System;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class RegistrationRequest : BaseRequest
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        
        public string FullName { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        public string ProfileImageLink { get; set; }
    }
}