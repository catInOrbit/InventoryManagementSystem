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

        public UserAndRole UserAndRole { get; set; }

        public PagingOption<UserAndRole> Paging { get; set; }

    }
    
    public class UserAndRole
    {
        public ApplicationUser ImsUser { get; set; }
        public string UserRole { get; set; }
        public string RoleID { get; set; }
        
    }
}