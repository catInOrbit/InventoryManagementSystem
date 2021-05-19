using System;

namespace InventoryManagementSystem.PublicApi.ResetPasswordEndpoints
{
    public class ResetPasswordSubmitResponse : BaseResponse
    {
        public ResetPasswordSubmitResponse(Guid correlationId) : base(correlationId)
        {
        }

        public ResetPasswordSubmitResponse()
        {
        }
        public bool Result { get; set; } = false;
        public string Username { get; set; } = string.Empty;
    }
}