using System;

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
        public bool IsLockedOut { get; set; } = false;
        public bool IsNotAllowed { get; set; } = false;
        public bool RequiresTwoFactor { get; set; } = false;
    }
}