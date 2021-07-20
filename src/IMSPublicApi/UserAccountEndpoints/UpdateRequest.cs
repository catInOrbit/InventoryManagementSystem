using System;

namespace InventoryManagementSystem.PublicApi.UserAccountEndpoints
{
    public class UpdateRequest : BaseRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageLink { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
    
    public class UpdateImageRequest : BaseRequest
    {
        public string ProfileImageLink { get; set; }
    }
}