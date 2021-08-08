using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class BaseSearchIndex : BaseEntity
    {
        [JsonIgnore]
        [NotMapped]
        public DateTime LatestUpdateDate { get; set; }
    }
}