using System.Collections;
using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Extensions
{

    public class StockTakeAdjustInfo
    {
        public string Id { get; set; }
        public List<StockTakeAdjustItemInfo> StockTakeAdjustItemsInfos { get; set; }
    }
    
    public class StockTakeAdjustItemInfo
    {
        public string StockTakeId { get; set; }
        public string StockTakeItemId { get; set; }
        public string PackageId { get; set; }
        public int QuantityToAdjust { get; set; }
    }
}