using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetPackageById : BaseAsyncEndpoint.WithRequest<PackageRequest>.WithResponse<PackageResponse>
    {

        private readonly IAsyncRepository<Package> _packageAsyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetPackageById(IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService)
        {
            _packageAsyncRepository = packageAsyncRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet("api/package/{PackageId}")]
        [SwaggerOperation(
            Summary = "Search Package by id",
            Description = "Search Package by id",
            OperationId = "package.id",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<PackageResponse>> HandleAsync([FromRoute]PackageRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Read))
                return Unauthorized();
            var response = new PackageResponse();

            response.Package = await _packageAsyncRepository.GetByIdAsync(request.PackageId);
            response.Package.IsDisplayingDetail = true;
            return Ok(response);
        }
    }
    
    
    public class SearchPackage : BaseAsyncEndpoint.WithRequest<PackageSearchRequest>.WithResponse<PackageResponse>
    {

        private readonly IAsyncRepository<Package> _packageAsyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public SearchPackage(IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _packageAsyncRepository = packageAsyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }

        
        [HttpGet("api/package")]
        [SwaggerOperation(
            Summary = "Search Packages",
            Description = "Search Packages",
            OperationId = "package.search",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<PackageResponse>> HandleAsync([FromQuery]PackageSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Read))
                return Unauthorized();
            var response = new PackageSearchResponse();
            
            PagingOption<Package> pagingOption =
                new PagingOption<Package>(request.CurrentPage, request.SizePerPage);
            response.IsDisplayingAll = true;
            
            if (request.SearchQuery == null)
            {
                response.Paging = await 
                    _packageAsyncRepository.GetPackages(pagingOption, cancellationToken);
                return Ok(response);
            }
            
            
            ISearchResponse<Package> responseElastic;

            if (!request.IsLocationOnly)
            {
                responseElastic = await _elasticClient.SearchAsync<Package>
                (
                    s => s.Size(2000).Index( ElasticIndexConstant.PACKAGES).
                        Query(q =>q.
                            QueryString(d =>d.Query('*' + request.SearchQuery + '*'))));    
            }

            else
            {
                responseElastic = await _elasticClient.SearchAsync<Package>
                (
                    s => s.Size(2000).Index( ElasticIndexConstant.PACKAGES).
                        Source(
                            source => source.Includes(
                                    fi => fi.Field(package => package.Location)
                                )
                        ).
                        Query(q =>q.
                            QueryString(d =>d.Query('*' + request.SearchQuery + '*'))));
            }
            
            pagingOption.ResultList = _packageAsyncRepository.PackageIndexFiltering(responseElastic.Documents.ToList(), request,
                new CancellationToken());
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
    
    
    
    
    
    
}