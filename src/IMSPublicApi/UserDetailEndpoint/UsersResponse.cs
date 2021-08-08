using System;
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
    
  
}