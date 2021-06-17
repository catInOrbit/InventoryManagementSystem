using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;

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

        public List<IdentityRole> Roles { get; set; }
    }
}