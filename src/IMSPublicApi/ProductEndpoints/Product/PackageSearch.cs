using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
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
            OperationId = "package.searchid",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<PackageResponse>> HandleAsync([FromRoute]PackageRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PACKAGE_SEARCH, UserOperations.Read))
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
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public SearchPackage(IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient, IAsyncRepository<ProductVariant> productVariantAsyncRepository)
        {
            _packageAsyncRepository = packageAsyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
            _productVariantAsyncRepository = productVariantAsyncRepository;
        }

        
        [HttpGet("api/package")]
        [SwaggerOperation(
            Summary = "Search Packages",
            Description = "Search Packages",
            OperationId = "package.searchall",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<PackageResponse>> HandleAsync([FromQuery]PackageSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PACKAGE_SEARCH, UserOperations.Read))
                return Unauthorized();
            
            PagingOption<Package> pagingOptionPackage =
                new PagingOption<Package>(request.CurrentPage, request.SizePerPage);
            
            PagingOption<ApplicationCore.Entities.Products.Location> pagingOptionLocation =
                new PagingOption<ApplicationCore.Entities.Products.Location>(request.CurrentPage, request.SizePerPage);
            
            
            ISearchResponse<Package> responseElastic;

            List<Package> resource;

            

            if (!request.IsLocationOnly)
            {
                
                var response = new PackageSearchResponse();
                response.IsDisplayingAll = true;
                
                ElasticSearchHelper<Package> elasticSearchHelper = new ElasticSearchHelper<Package>(_elasticClient, request.SearchQuery,
                    ElasticIndexConstant.PACKAGES);
                
                resource = (await elasticSearchHelper.GetDocuments()).Documents.ToList();

                pagingOptionPackage.ResultList = FilteringService.PackageIndexFiltering(resource, request,
                    new CancellationToken());

                foreach (var package in pagingOptionPackage.ResultList)
                {
                    package.IsShowingProductVariant = true;
                    package.ProductVariant = await _productVariantAsyncRepository.GetByIdAsync(package.ProductVariantId);
                    package.ProductVariant.IsShowingPackage = false;
                    package.ProductVariant.IsShowingTransaction = false;
                }
            
                pagingOptionPackage.ExecuteResourcePaging();
                response.Paging = pagingOptionPackage;
                return Ok(response);
            }

            else
            {
                // responseElastic = await _elasticClient.SearchAsync<Package>
                // (
                //     s => s.Size(2000).Index( ElasticIndexConstant.PACKAGES).
                //         Source(
                //             source => source.Includes(
                //                 fi => fi.Field(package => package.Location)
                //             )
                //         ).Sort(ss => ss.Descending(p => p.LatestUpdateDate)).
                //         Query(q =>q.
                //             QueryString(d =>d.Query('*' + request.SearchQuery + '*'))));
                ISearchResponse<ApplicationCore.Entities.Products.Location> responseElasticLocation;

                ElasticSearchHelper<ApplicationCore.Entities.Products.Location> elasticSearchHelper = new ElasticSearchHelper<ApplicationCore.Entities.Products.Location>(_elasticClient, request.SearchQuery,
                    ElasticIndexConstant.LOCATIONS);
                
                responseElasticLocation = await _elasticClient.SearchAsync<ApplicationCore.Entities.Products.Location>
                (
                    s => s.Size(2000).Index( ElasticIndexConstant.LOCATIONS).
                       Sort(ss => ss.Descending(p => p.LatestUpdateDate)).
                        Query(q =>q.
                            QueryString(d =>d.Query('*' + request.SearchQuery + '*'))));
                // responseElasticLocation = await elasticSearchHelper.GetDocuments();

                
                // var locations = responseElasticLocation.Documents.Where(x => x.Location!= null).Select(x => x.Location).ToList();
                // locations = locations.GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToList();
                pagingOptionLocation.ResultList = responseElasticLocation.Documents.ToList();
                pagingOptionLocation.ExecuteResourcePaging(pagingOptionLocation.ResultList.Count);
                
                var response = new LocationSearchResponse();
                response.IsDisplayingAll = true;

                response.Paging = pagingOptionLocation;
                return Ok(response);
            }

        }
    }
    
    
    
    
    
    
}