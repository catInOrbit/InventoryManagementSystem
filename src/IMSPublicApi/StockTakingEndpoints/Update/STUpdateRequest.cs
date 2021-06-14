using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Update
{
    public class STUpdateRequest : BaseRequest
    {
        public List<StockTakeItem> StockTakeItems { get; set; }
        public string StockTakeId { get; set; }
    }
}