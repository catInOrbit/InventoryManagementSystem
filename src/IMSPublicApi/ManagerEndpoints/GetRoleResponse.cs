using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;

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

        public List<ApplicationRole> Roles { get; set; }
    }
}