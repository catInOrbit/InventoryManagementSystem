using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RolePermissionRequest : BaseRequest
    {
        public string Role { get; set; }
        public string RoleDescription { get; set; }
        public Dictionary<string, List<string>> PageClaimDictionary { get; set; }
    }
}