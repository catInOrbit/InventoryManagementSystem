using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Reports;

namespace InventoryManagementSystem.PublicApi.ReportEndpoints
{
    public class StockOnHandReportResponse
    {
        public PagingOption<StockOnhandReport> Paging { get; set; }
    }
    
    public class StockTakeReportResponse
    {
        public PagingOption<StockTakeReport> Paging { get; set; }
    }
    
    public class TopSellingResponse
    {
        public PagingOption<TopSellingReport> Paging { get; set; }
    }
    
    public class TrainMLResponse
    {
        public string TopSellingProductName { get; set; }
    }
    
    public class InventoryCostResponse
    {
        public decimal InventoryCostThisMonth { get; set; }
    }
    
    public class QuantityThisMonthResponse
    {
        public int SumInventoryCountThisMonth { get; set; }
    }
    
    public class ImportExportThisMonthResponse
    {
        public int ImportQuantity { get; set; }
        public int ExportQuantity { get; set; }

    }
    
    public class StockReportRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}