using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReportEndpoints
{
    public class GenerateStockOnHandReport : BaseAsyncEndpoint.WithRequest<StockOnHandReportRequest>.WithResponse<StockOnHandReportResponse>
    {
        private IAsyncRepository<Package> _packageAsycnRepository;
        private readonly IAuthorizationService _authorizationService;

        public GenerateStockOnHandReport(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
        {
            _packageAsycnRepository = packageAsycnRepository;
            _authorizationService = authorizationService;
        }

        
        [HttpGet]
        [Route("api/report/onhand")]
        [SwaggerOperation(
            Summary = "Create a stock on hand report",
            Description = "Create a stock on hand report",
            OperationId = "report.onhand",
            Tags = new[] { "ReportEndpoints" })
        ]
        public override async Task<ActionResult<StockOnHandReportResponse>> HandleAsync([FromQuery]StockOnHandReportRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Create))
               return Unauthorized();
            
            var response = new StockOnHandReportResponse();
            PagingOption<StockOnhandReport> pagingOption =
                new PagingOption<StockOnhandReport>(request.CurrentPage, request.SizePerPage);
            response.Paging = await _packageAsycnRepository.GenerateOnHandReport(pagingOption, cancellationToken);
            return Ok(response);
        }
    }
    
    public class GenerateStockTakeReport : BaseAsyncEndpoint.WithRequest<StockOnHandReportRequest>.WithResponse<StockOnHandReportResponse>
    {
        private IAsyncRepository<Package> _packageAsycnRepository;
        private readonly IAuthorizationService _authorizationService;

        public GenerateStockTakeReport(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
        {
            _packageAsycnRepository = packageAsycnRepository;
            _authorizationService = authorizationService;
        }

        
        [HttpGet]
        [Route("api/report/stocktake")]
        [SwaggerOperation(
            Summary = "Create a stock take report",
            Description = "Create a stock take report",
            OperationId = "report.stocktake",
            Tags = new[] { "ReportEndpoints" })
        ]
        public override async Task<ActionResult<StockOnHandReportResponse>> HandleAsync([FromQuery]StockOnHandReportRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Create))
                return Unauthorized();
            
            var response = new StockTakeReportResponse();
            
            PagingOption<StockTakeReport> pagingOption =
                new PagingOption<StockTakeReport>(request.CurrentPage, request.SizePerPage);
            
            response.Paging = await _packageAsycnRepository.GenerateStockTakeReport(pagingOption, cancellationToken);
            return Ok(response);
        }
    }
}