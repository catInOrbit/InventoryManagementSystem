using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.Reports;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
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
        [Route("api/report/inventorycost")]
        [SwaggerOperation(
            Summary = "Create a stock inventory cost report",
            Description = "Create an inventory cost  report",
            OperationId = "report.inventorycost",
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
    
    // public class GenerateTopSellingProductThisYear : BaseAsyncEndpoint.WithRequest<StockReportRequest>.WithResponse<TopSellingResponse>
    // {
    //     private IAsyncRepository<Package> _packageAsycnRepository;
    //     private readonly IAuthorizationService _authorizationService;
    //
    //     public GenerateTopSellingProductThisYear(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
    //     {
    //         _packageAsycnRepository = packageAsycnRepository;
    //         _authorizationService = authorizationService;
    //     }
    //     
    //     [HttpGet]
    //     [Route("api/report/topselling/currentyear")]
    //     [SwaggerOperation(
    //         Summary = "Create a stock take report",
    //         Description = "Create a stock take report",
    //         OperationId = "report.currentyear",
    //         Tags = new[] { "ReportEndpoints" })
    //     ]
    //     public override async Task<ActionResult<TopSellingResponse>> HandleAsync([FromQuery]StockReportRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
    //             return Unauthorized();
    //         
    //         var response = new TopSellingResponse();
    //         
    //         PagingOption<TopSellingReport> pagingOption =
    //             new PagingOption<TopSellingReport>(request.CurrentPage, request.SizePerPage);
    //         
    //         response.Paging = await _packageAsycnRepository.GenerateTopSellingReport(ReportType.Year,pagingOption, cancellationToken);
    //
    //         return Ok(response);
    //     }
    // }
    //
    // public class GenerateTopSellingProductThisMonth : BaseAsyncEndpoint.WithRequest<StockReportRequest>.WithResponse<TopSellingResponse>
    // {
    //     private IAsyncRepository<Package> _packageAsycnRepository;
    //     private readonly IAuthorizationService _authorizationService;
    //
    //     public GenerateTopSellingProductThisMonth(IAsyncRepository<Package> packageAsycnRepository, IAuthorizationService authorizationService)
    //     {
    //         _packageAsycnRepository = packageAsycnRepository;
    //         _authorizationService = authorizationService;
    //     }
    //     
    //     [HttpGet]
    //     [Route("api/report/topselling/currentmonth")]
    //     [SwaggerOperation(
    //         Summary = "Create a stock take report",
    //         Description = "Create a stock take report",
    //         OperationId = "report.currentmonth",
    //         Tags = new[] { "ReportEndpoints" })
    //     ]
    //     public override async Task<ActionResult<TopSellingResponse>> HandleAsync([FromQuery]StockReportRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
    //             return Unauthorized();
    //         
    //         var response = new TopSellingResponse();
    //         
    //         PagingOption<TopSellingReport> pagingOption =
    //             new PagingOption<TopSellingReport>(request.CurrentPage, request.SizePerPage);
    //         
    //         response.Paging = await _packageAsycnRepository.GenerateTopSellingReport(ReportType.Month,pagingOption, cancellationToken);
    //         return Ok(response);
    //     }
    //     
    //     
    // }
    
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
            OperationId = "report.trainml",
            Tags = new[] { "ReportEndpoints" })
        ]
        public override async Task<ActionResult<TrainMLResponse>> HandleAsync( CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
                return Unauthorized();
    
            BigQueryService bqs = new BigQueryService();
            var response = new TrainMLResponse();
    
            response.TopSellingProductName = await bqs.TrainMLWithLargestSoldProduct();

            GoogleSheetService googleSheetService = new GoogleSheetService();
            googleSheetService.OverrideForecastDatasheet();
    
            return Ok(response);
        }
        
        
    }
    
    // public class OnHandSumReport : BaseAsyncEndpoint.WithoutRequest.WithResponse<InventoryCostResponse>
    // {
    //     private IAsyncRepository<Package> _packageAsyncRepository;
    //     private readonly IAuthorizationService _authorizationService;
    //
    //     public OnHandSumReport(IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService)
    //     {
    //         _packageAsyncRepository = packageAsyncRepository;
    //         _authorizationService = authorizationService;
    //     }
    //
    //     [HttpGet]
    //     [Route("api/report/inventorycostsum")]
    //     [SwaggerOperation(
    //         Summary = "Create a Inventory cost sum report",
    //         Description = "Create a Inventory cost report",
    //         OperationId = "report.inventorycostsum",
    //         Tags = new[] { "ReportEndpoints" })
    //     ]
    //     public override async Task<ActionResult<InventoryCostResponse>> HandleAsync( CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
    //             return Unauthorized();
    //
    //         var response = new InventoryCostResponse();
    //         ReportBusinessService rbs = new ReportBusinessService();
    //         response.InventoryCostThisMonth = await rbs.CalculateInventoryCostThisMonth(_packageAsyncRepository);
    //         return Ok(response);
    //     }
    // }
    
    // public class QuantityOfAllProductInSystem : BaseAsyncEndpoint.WithoutRequest.WithResponse<QuantityThisMonthResponse>
    // {
    //     private IAsyncRepository<Package> _packageAsyncRepository;
    //     private readonly IAuthorizationService _authorizationService;
    //
    //     public QuantityOfAllProductInSystem(IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService)
    //     {
    //         _packageAsyncRepository = packageAsyncRepository;
    //         _authorizationService = authorizationService;
    //     }
    //
    //     [HttpGet]
    //     [Route("api/report/sumquantityproduct")]
    //     [SwaggerOperation(
    //         Summary = "Create a Inventory report of all product quantity",
    //         Description = "Create a Inventory report of all product quantity",
    //         OperationId = "report.sumquantityproduct",
    //         Tags = new[] { "ReportEndpoints" })
    //     ]
    //     public override async Task<ActionResult<QuantityThisMonthResponse>> HandleAsync( CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
    //             return Unauthorized();
    //
    //       
    //         return Ok(response);
    //     }
    // }
    
    
    // public class ImportExportProductThisMonth : BaseAsyncEndpoint.WithoutRequest.WithResponse<ImportExportThisMonthResponse>
    // {
    //     private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
    //     private IAsyncRepository<GoodsIssueOrder> _giAsyncRepository;
    //
    //     private readonly IAuthorizationService _authorizationService;
    //
    //     public ImportExportProductThisMonth(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<GoodsIssueOrder> giAsyncRepository, IAuthorizationService authorizationService)
    //     {
    //         _roAsyncRepository = roAsyncRepository;
    //         _giAsyncRepository = giAsyncRepository;
    //         _authorizationService = authorizationService;
    //     }
    //
    //
    //     [HttpGet]
    //     [Route("api/report/importexportquantity")]
    //     [SwaggerOperation(
    //         Summary = "Create an Inventory report of import and export quantity this month",
    //         Description = "Create a Inventory report of import and export quantity this month",
    //         OperationId = "report.importexportquantity",
    //         Tags = new[] { "ReportEndpoints" })
    //     ]
    //     public override async Task<ActionResult<ImportExportThisMonthResponse>> HandleAsync( CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
    //             return Unauthorized();
    //
    //         var response = new ImportExportThisMonthResponse();
    //         ReportBusinessService rbs = new ReportBusinessService();
    //         var result = await rbs.CalculateImportExportQuantityThisMonth(_roAsyncRepository, _giAsyncRepository);
    //         
    //         foreach (var keyValuePair in result)
    //         {
    //             response.ImportQuantity = keyValuePair.Key;
    //             response.ExportQuantity = keyValuePair.Value;
    //         }
    //         return Ok(response);
    //     }
    // }
    
    public class GenerateDashboardInfo : BaseAsyncEndpoint.WithoutRequest.WithResponse<DashboardData>
    {
        private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private IAsyncRepository<GoodsIssueOrder> _giAsyncRepository;
        private IAsyncRepository<Package> _packageAsyncRepository;

        private readonly IAuthorizationService _authorizationService;

        public GenerateDashboardInfo(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<GoodsIssueOrder> giAsyncRepository, IAuthorizationService authorizationService, IAsyncRepository<Package> packageAsyncRepository)
        {
            _roAsyncRepository = roAsyncRepository;
            _giAsyncRepository = giAsyncRepository;
            _authorizationService = authorizationService;
            _packageAsyncRepository = packageAsyncRepository;
        }


        [HttpGet]
        [Route("api/report/dashboarddata")]
        [SwaggerOperation(
            Summary = "Create an Inventory report of import and export quantity this month",
            Description = "Create a Inventory report of import and export quantity this month",
            OperationId = "report.importexportquantity",
            Tags = new[] { "ReportEndpoints" })
        ]
        public override async Task<ActionResult<DashboardData>> HandleAsync( CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.REPORT, UserOperations.Read))
                return Unauthorized();

            
            var response = new DashboardData();
            ReportBusinessService rbs = new ReportBusinessService();
            var result = await rbs.CalculateImportExportQuantityThisMonth(_roAsyncRepository, _giAsyncRepository);
            
            foreach (var keyValuePair in result)
            {
                response.ImportQuantity = keyValuePair.Key;
                response.ExportQuantity = keyValuePair.Value;
            }
            
            response.SumInventoryCountThisMonth = await rbs.CalculateInventoryQuantityOfAllProducts(_packageAsyncRepository);
            response.InventoryCostThisMonth = await rbs.CalculateInventoryCostThisMonth(_packageAsyncRepository);
            response.TopSellingMonthPaging = await _packageAsyncRepository.GenerateTopSellingReport(ReportType.Month,new PagingOption<TopSellingReport>(1,5), cancellationToken);
            response.TopSellingYearPaging = await _packageAsyncRepository.GenerateTopSellingReport(ReportType.Year,new PagingOption<TopSellingReport>(1,5), cancellationToken);
            return Ok(response);
        }
    }
    
}