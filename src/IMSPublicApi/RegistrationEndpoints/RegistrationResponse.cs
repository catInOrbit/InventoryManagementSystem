using System;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.RegistrationEndpoints
{
    public class RegistrationResponse : BaseResponse
    {
        public RegistrationResponse(Guid correlationId) : base(correlationId)
        {
        }

        public RegistrationResponse()
        {
        }
        
        public bool Result { get; set; } = false;
        public string Verbose { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        
        public UserAndRole UserAndRole { get; set; }
    }
}