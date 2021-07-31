using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class DeactivateUserRequest : BaseRequest
    {
        public string UserId { get; set; }
        public bool IsDeactivated { get; set; }
    }
    
    public class DeactivateUserResponse : BaseResponse
    {
        public UserAndRole UserAndRole { get; set; }
    }
}