using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RolePermissionRequest : BaseRequest
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }
        public Dictionary<string, List<string>> PageClaimDictionary { get; set; }
        // public List<PagePermissions> PagePermissions { get; set; } = new List<PagePermissions>();
    }
    
    public class RolePermissionCreateRequest : BaseRequest
    {
        public string RoleName { get; set; }
        public Dictionary<string, List<string>> PageClaimDictionary { get; set; }
        // public List<PagePermissions> PagePermissions { get; set; } = new List<PagePermissions>();
    }
}