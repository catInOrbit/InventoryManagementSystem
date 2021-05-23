using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class UserInfoAuth
    {
        public static readonly UserInfoAuth Anonymous = new UserInfoAuth();
        public bool IsAuthenticated { get; set; }
        public string NameClaimType { get; set; }
        public string RoleClaimType { get; set; }
        public IEnumerable<ClaimValue> Claims { get; set; }
    }
}
