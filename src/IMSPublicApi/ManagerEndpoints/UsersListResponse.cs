using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class UsersListResponse : BaseResponse
    {
        public UsersListResponse(Guid correlationId) : base(correlationId)
        {
        }

        public UsersListResponse()
        {
        }

        public List<UserTestDTO> UserListDTO { get; set; } = new List<UserTestDTO>();
    }
}