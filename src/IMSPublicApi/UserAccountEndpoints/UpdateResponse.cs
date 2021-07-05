using System;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.UserAccountEndpoints
{
    public class UpdateResponse : BaseResponse
    {
        public UpdateResponse(Guid correlationId) : base(correlationId)
        {
        }

        public UpdateResponse()
        {
        }
        
        public bool Result { get; set; } = false;
        public string Verbose { get; set; }
        
        public ApplicationUser ApplicationUser { get; set; }
    }
}