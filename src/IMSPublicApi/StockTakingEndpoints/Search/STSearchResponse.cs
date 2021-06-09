using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Search
{
    public class STSearchResponse : BaseResponse
    {
        public List<StockTakeSearchIndex> StockTakeSearchIndices { get; set; } =
            new List<StockTakeSearchIndex>();

        public StockTakeOrder StockTakeOrder { get; set; }
        
        
        [JsonIgnore]
        public bool IsDisplayingAll { get; set; }
        
        public bool ShouldSerializeStockTakeSearchIndices()
        {
            if (IsDisplayingAll)
                return true;
            return false;
        }

    }
}