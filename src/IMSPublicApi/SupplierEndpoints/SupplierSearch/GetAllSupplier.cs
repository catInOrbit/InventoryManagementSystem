using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.SupplierSearch
{
    // public class GetAllSupplier : BaseAsyncEndpoint.WithRequest<GetAllSupplierRequest>.WithResponse<GetSupplierResponse>
    // {
    //     private IAsyncRepository<Supplier> _asyncRepository;
    //     private readonly IAuthorizationService _authorizationService;
    //
    //     public GetAllSupplier(IAsyncRepository<Supplier> asyncRepository, IAuthorizationService authorizationService)
    //     {
    //         _asyncRepository = asyncRepository;
    //         _authorizationService = authorizationService;
    //     }
    //     
    //     [HttpPost("api/suppliers/all")]
    //     [SwaggerOperation(
    //         Summary = "Get all supplier",
    //         Description = "Get all supplier"  +
    //                       "\n {SearchQuery}: Querry to search, all to search all \n " +
    //                       "{CurrentPage}: Current page to display \n" +
    //                       "{SizePerPage}: Number of rows to display in a page \n " +
    //                       "{Status} Status of purchase order",
    //         OperationId = "sups.update",
    //         Tags = new[] { "SupplierEndpoints" })
    //     ]
    //     public override async Task<ActionResult<GetSupplierResponse>> HandleAsync(GetAllSupplierRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.SUPPLIER, UserOperations.Read))
    //             return Unauthorized();
    //         
    //         var response = new GetSupplierResponse();
    //         PagingOption<Supplier> pagingOption =
    //             new PagingOption<Supplier>(request.CurrentPage, request.SizePerPage);
    //         response.IsDisplayingAll = true;
    //         
    //         response.Paging = await 
    //             _asyncRepository.ListAllAsync(pagingOption, cancellationToken);
    //  
    //         return Ok(response);
    //     }
    // }
    //
     public class SearchSupplier : BaseAsyncEndpoint.WithRequest<SupplierSearchRequest>.WithResponse<GetSupplierResponse>
    {
        private IAsyncRepository<Supplier> _asyncRepository;
        private readonly IElasticClient _elasticClient;
        private readonly IAuthorizationService _authorizationService;

        public SearchSupplier(IAsyncRepository<Supplier> asyncRepository, IElasticClient elasticClient, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _elasticClient = elasticClient;
            _authorizationService = authorizationService;
        }
        
        [HttpGet("api/suppliers/search")]
        [SwaggerOperation(
            Summary = "Get all supplier",
            Description = "Get all supplier"  +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page \n " +
                          "{Status} Status of purchase order",
            OperationId = "sups.update",
            Tags = new[] { "SupplierEndpoints" })
        ]
        public override async Task<ActionResult<GetSupplierResponse>> HandleAsync([FromQuery]SupplierSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.SUPPLIER_SEARCH, UserOperations.Read))
                return Unauthorized();
            
            var response = new GetSupplierResponse();
            PagingOption<Supplier> pagingOption =
                new PagingOption<Supplier>(request.CurrentPage, request.SizePerPage);
            response.IsDisplayingAll = true;

            
            ISearchResponse<Supplier> responseElastic;
      
            ElasticSearchHelper<Supplier> elasticSearchHelper = new ElasticSearchHelper<Supplier>(_elasticClient, request.SearchQuery,
                ElasticIndexConstant.SUPPLIERS);
            responseElastic = await elasticSearchHelper.GetDocuments();                        

            foreach (var purchaseOrderSearchIndex in responseElastic.Documents)
            {
                pagingOption.ResultList.Add(purchaseOrderSearchIndex);
            }
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
}