using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.UserDetailEndpoint
{
    public class UsersResponse : BaseResponse
    {
        public UsersResponse(Guid correlationId) : base(correlationId)
        { }

        public UsersResponse()
        { }

        public List<ApplicationUser> ImsUser { get; set; }
        public string UserRole { get; set; }
    }
}