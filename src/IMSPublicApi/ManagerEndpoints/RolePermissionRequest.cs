using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RolePermissionRequest : BaseRequest
    {
        public string Role { get; set; }
        public string Page { get; set; }

        public Dictionary<string, string> PermissionClaimValues { get; set; }
    }
}