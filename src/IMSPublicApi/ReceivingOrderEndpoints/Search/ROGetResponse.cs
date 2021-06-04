using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
    public class ROGetResponse : BaseResponse
    {
        public ROGetResponse(Guid correlationId) : base()
        {
            base._correlationId = correlationId;
        }

        public ROGetResponse()
        { }

        public List<ReceiveingOrderSearchIndex> ReceiveingOrderSearchIndex { get; set; } = new List<ReceiveingOrderSearchIndex>();
    }
}