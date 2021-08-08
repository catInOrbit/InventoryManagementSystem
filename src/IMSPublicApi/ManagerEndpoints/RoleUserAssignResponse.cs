using System;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RoleUserAssignResponse : BaseResponse
    {
        public RoleUserAssignResponse(Guid correlationId) : base(correlationId)
        {}

        public RoleUserAssignResponse()
        { }

        public bool Result { get; set; }
    }
}