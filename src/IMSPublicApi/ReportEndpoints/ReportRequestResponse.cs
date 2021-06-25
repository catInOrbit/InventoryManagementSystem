using System;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.PublicApi.ReportEndpoints
{
    public class StockOnHandReportResponse
    {
        public DateTime Date { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}