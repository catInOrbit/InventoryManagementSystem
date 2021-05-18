using System;

namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class ResetPasswordResponse : BaseResponse
    {
        public ResetPasswordResponse(Guid correlationId) : base(correlationId)
        {
        }

        public ResetPasswordResponse()
        {
        }
        public bool Result { get; set; } = false;
    }
}