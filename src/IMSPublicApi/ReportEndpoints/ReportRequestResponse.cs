using System;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

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
    
    public class StockOnHandReportRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
}