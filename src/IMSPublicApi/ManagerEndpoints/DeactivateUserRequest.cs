namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class DeactivateUserRequest : BaseRequest
    {
        public string UserId { get; set; }
        public bool IsDeactivated { get; set; }
    }
}