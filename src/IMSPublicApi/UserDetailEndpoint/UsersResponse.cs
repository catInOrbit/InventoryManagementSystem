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

        public List<IMSUser> ImsUser { get; set; }
    }
}