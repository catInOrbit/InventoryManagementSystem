using System;

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
        public string RoleChanged { get; set; }
    }
}