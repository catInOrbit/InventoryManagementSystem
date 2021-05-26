using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class GetRoleResponse : BaseResponse
    {
        public GetRoleResponse(Guid correlationId) : base(correlationId)
        {
                
        }

        public GetRoleResponse()
        {
            
        }

        public List<string> Roles { get; set; }
    }
}