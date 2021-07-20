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
    
    public class StockReportRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}