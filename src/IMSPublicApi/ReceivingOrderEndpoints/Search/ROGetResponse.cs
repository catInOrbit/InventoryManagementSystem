﻿using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
    public class ROGetResponse : BaseResponse
    {
        public ROGetResponse(Guid correlationId) : base()
        {
            base._correlationId = correlationId;
        }
        
        [JsonIgnore]
        public bool IsDislayingAll { get; set; }

        public bool ShouldSerializeReceiveingOrderSearchIndex()
        {
            if (IsDislayingAll) 
                return true;
            return false;
        }

        public ROGetResponse()
        { }

        public ReceivingOrder ReceiveingOrder { get; set; }

        public List<ReceivingOrderSearchIndex> ReceiveingOrderSearchIndex { get; set; } = new List<ReceivingOrderSearchIndex>();
    }
}