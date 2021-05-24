namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RoleUserAssignRequest : BaseRequest
    {
        public string UserID { get; set; }
        
        public string NewRole { get; set; }

    }
}