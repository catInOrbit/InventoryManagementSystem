namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
    public class UsersRequest : BaseRequest
    {
        public string UserID { get; set; }
    }
    
    public class GetAllUserRequest : BaseRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}