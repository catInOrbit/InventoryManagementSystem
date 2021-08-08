namespace InventoryManagementSystem.PublicApi.UserAccountEndpoints
{
    public class PasswordUpdateRequest : BaseRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
}