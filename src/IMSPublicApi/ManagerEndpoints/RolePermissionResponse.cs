using System;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RolePermissionResponse : BaseResponse
    {
        public RolePermissionResponse(Guid correlationID) : base(correlationID)
        { }

        public RolePermissionResponse()
        { }

        public bool Result { get; set; } = false;
        public string Verbose { get; set; }
        public IdentityRole RoleChanged { get; set; }
    }
}