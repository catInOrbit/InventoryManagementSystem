using System;

namespace InventoryManagementSystem.PublicApi.ResetPasswordEndpoints
{
    public class ResetPasswordLeadResponse : BaseResponse
    {
        public ResetPasswordLeadResponse(Guid correlationId) : base(correlationId)
        {
        }

        public ResetPasswordLeadResponse()
        {
        }
        public bool Result { get; set; } = false;
    }
}