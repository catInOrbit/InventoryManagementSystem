using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Google.Cloud.BigQuery.V2;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.Reports;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReportEndpoints
{
    public class GenerateStockOnHandReport : BaseAsyncEndpoint.WithRequest<StockReportRequest>.WithResponse<StockOnHandReportResponse>
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
        public override async Task<ActionResult<StockOnHandReportResponse>> HandleAsync([FromQuery]StockReportRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
               return Unauthorized();
            
            var response = new StockOnHandReportResponse();
            PagingOption<StockOnhandReport> pagingOption =
                new PagingOption<StockOnhandReport>(request.CurrentPage, request.SizePerPage);
            response.Paging = await _packageAsycnRepository.GenerateOnHandReport(pagingOption, cancellationToken);
            return Ok(response);
        }
    }
    
    public class GenerateStockTakeReport : BaseAsyncEndpoint.WithRequest<StockReportRequest>.WithResponse<StockTakeReportResponse>
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
        public override async Task<ActionResult<StockTakeReportResponse>> HandleAsync([FromQuery]StockReportRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
                return Unauthorized();
            
            var response = new StockTakeReportResponse();
            
            PagingOption<StockTakeReport> pagingOption =
                new PagingOption<StockTakeReport>(request.CurrentPage, request.SizePerPage);
            
            response.Paging = await _packageAsycnRepository.GenerateStockTakeReport(pagingOption, cancellationToken);
            return Ok(response);
        }
    }
    
    public class GenerateTopSellingProductThisYear : BaseAsyncEndpoint.WithRequest<StockReportRequest>.WithResponse<TopSellingResponse>
    {
        private IAsyncRepository<Package> _packageAsycnRepository;
        private readonly IAuthorizationService _authorizationService;
    
        public GenerateTopSellingProductThisYear(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
        {
            _packageAsycnRepository = packageAsycnRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet]
        [Route("api/report/topselling/currentyear")]
        [SwaggerOperation(
            Summary = "Create a stock take report",
            Description = "Create a stock take report",
            OperationId = "report.stocktake",
            Tags = new[] { "ReportEndpoints" })
        ]
        public override async Task<ActionResult<TopSellingResponse>> HandleAsync([FromQuery]StockReportRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
                return Unauthorized();
            
            var response = new TopSellingResponse();
            
            PagingOption<TopSellingReport> pagingOption =
                new PagingOption<TopSellingReport>(request.CurrentPage, request.SizePerPage);
            
            response.Paging = await _packageAsycnRepository.GenerateTopSellingReport(ReportType.Year,pagingOption, cancellationToken);

            return Ok(response);
        }
    }
    
    public class GenerateTopSellingProductThisMonth : BaseAsyncEndpoint.WithRequest<StockReportRequest>.WithResponse<TopSellingResponse>
    {
        private IAsyncRepository<Package> _packageAsycnRepository;
        private readonly IAuthorizationService _authorizationService;

        public GenerateTopSellingProductThisMonth(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
        {
            _packageAsycnRepository = packageAsycnRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet]
        [Route("api/report/topselling/currentmonth")]
        [SwaggerOperation(
            Summary = "Create a stock take report",
            Description = "Create a stock take report",
            OperationId = "report.stocktake",
            Tags = new[] { "ReportEndpoints" })
        ]
        public override async Task<ActionResult<TopSellingResponse>> HandleAsync([FromQuery]StockReportRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
                return Unauthorized();
            
            var response = new TopSellingResponse();
            
            PagingOption<TopSellingReport> pagingOption =
                new PagingOption<TopSellingReport>(request.CurrentPage, request.SizePerPage);
            
            response.Paging = await _packageAsycnRepository.GenerateTopSellingReport(ReportType.Month,pagingOption, cancellationToken);
            return Ok(response);
        }
        
        
    }
    
    public class TrainMLAndGetLatest : BaseAsyncEndpoint.WithoutRequest.WithResponse<TrainMLResponse>
    {
        private IAsyncRepository<Package> _packageAsycnRepository;
        private readonly IAuthorizationService _authorizationService;

        public TrainMLAndGetLatest(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
        {
            _packageAsycnRepository = packageAsycnRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet]
        [Route("api/report/trainml")]
        [SwaggerOperation(
            Summary = "Create a stock take report",
            Description = "Create a stock take report",
            OperationId = "report.stocktake",
            Tags = new[] { "ReportEndpoints" })
        ]
        public override async Task<ActionResult<TrainMLResponse>> HandleAsync( CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
                return Unauthorized();

            BigQueryService bqs = new BigQueryService();
            var response = new TrainMLResponse();

            response.TopSellingProductName = bqs.TrainMLWithLargestSoldProduct();
    
            return Ok(response);
        }
        
        
    }
    
    // public class GenerateMainSummary : BaseAsyncEndpoint.WithRequest<StockReportRequest>.WithResponse<TopSellingResponse>
    // {
    //     private IAsyncRepository<Package> _packageAsycnRepository;
    //     private readonly IAuthorizationService _authorizationService;
    //
    //     public GenerateMainSummary(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
    //     {
    //         _packageAsycnRepository = packageAsycnRepository;
    //         _authorizationService = authorizationService;
    //     }
    //     
    //     [HttpGet]
    //     [Route("api/report/mainsum")]
    //     [SwaggerOperation(
    //         Summary = "Create a main summary",
    //         Description = "Create a main summary",
    //         OperationId = "report.stocktake",
    //         Tags = new[] { "ReportEndpoints" })
    //     ]
    //     public override async Task<ActionResult<TopSellingResponse>> HandleAsync([FromQuery]StockReportRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
    //             return Unauthorized();
    //         
    //         var response = new TopSellingResponse();
    //
    //         BigQueryService bigQueryService = new BigQueryService();
    //         var result = bigQueryService.Get3LinesData();
    //         // foreach (var bigQueryResult in result)
    //         // {
    //         //     bigQueryResult.
    //         // }
    //         //
    //         // var stream = new MemoryStream(Encoding.ASCII.GetBytes())
    //       
    //         
    //         return Ok(response);
    //     }
    // }
    //
    
    
    
}