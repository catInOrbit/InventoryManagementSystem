using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.PublicApi.ReportEndpoints
{
    public class GenerateStockOnHandReport : BaseAsyncEndpoint.WithoutRequest.WithResponse<StockOnHandReportResponse>
    {
        public override Task<ActionResult<StockOnHandReportResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}