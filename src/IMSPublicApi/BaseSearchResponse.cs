using System;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi
{
    public abstract class BaseSearchResponse<T> : BaseMessage where T : class
    {
        public BaseSearchResponse(Guid correlationId) : base()
        {
            base._correlationId = correlationId;
        }

        public BaseSearchResponse()
        {
            
        }

        public bool ShouldSerializePaging()
        {
            if (IsDisplayingAll)
                return true;
            return false;
        }

        public bool ShouldSerializeSingleResult()
        {
            if (IsDisplayingAll)
                return false;
            return true;
        }

        [JsonIgnore]
        public bool IsDisplayingAll { get; set; }
        
        public PagingOption<T> Paging { get; set; }
        public T SingleResult { get; set; }
    }
}