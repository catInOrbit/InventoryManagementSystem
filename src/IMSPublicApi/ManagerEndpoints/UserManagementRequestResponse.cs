using System;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class UserManagementRequest : BaseRequest
    {
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string RoleId { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    
    public class UserManagementResponse : BaseResponse
    {
        public bool Status { get; set; }
        public string Verbose { get; set; }
    }
}