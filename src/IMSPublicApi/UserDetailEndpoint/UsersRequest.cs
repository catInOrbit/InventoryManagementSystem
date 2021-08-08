using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
    public class UsersRequest : BaseRequest
    {
        public string UserID { get; set; }
    }
    
    public class GetAllUserRequest : UserInfoFilter
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}