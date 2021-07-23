using System;
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
    // public class GetProduct : BaseAsyncEndpoint.WithRequest<GetProductAllRequest>.WithResponse<GetProductSearchResponse>
    // {
    //     private readonly IElasticClient _elasticClient;
    //     private readonly IAuthorizationService _authorizationService;
    //     private readonly IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
    //
    //     public GetProduct(IElasticClient elasticClient, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository)
    //     {
    //         _elasticClient = elasticClient;
    //         _authorizationService = authorizationService;
    //         _asyncRepository = asyncRepository;
    //     }
    //
    //     [HttpPost("api/product/all/")]
    //     [SwaggerOperation(
    //         Summary = "Search Product by Name",
    //         Description = "Search Product by Id",
    //         OperationId = "catalog-items.create",
    //         Tags = new[] { "ProductEndpoints" })
    //     ]
    //
    //     public override async Task<ActionResult<GetProductSearchResponse>> HandleAsync(GetProductAllRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
    //         //     return Unauthorized();
    //         var response = new GetProductSearchResponse();
    //
    //         PagingOption<ProductSearchIndex> pagingOption =
    //             new PagingOption<ProductSearchIndex>(request.CurrentPage, request.SizePerPage);
    //         
    //             response.IsDisplayingAll = true;
    //             response.Paging = await 
    //                 _asyncRepository.GetProductForELIndexAsync(pagingOption, cancellationToken);
    //
    //         return Ok(response);
    //     }
    // }
    //
    public class SearchProductVariant : BaseAsyncEndpoint.WithRequest<GetProductVariantSearchRequest>.WithResponse<GetProductVariantSearchResponse>
    {
        private readonly IElasticClient _elasticClient;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ProductVariant> _asyncRepository;

        public SearchProductVariant(IElasticClient elasticClient, IAuthorizationService authorizationService, IAsyncRepository<ProductVariant> asyncRepository)
        {
            _elasticClient = elasticClient;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
        }

        [HttpGet("api/productvariant/search")]
        [SwaggerOperation(
            Summary = "Search Product Variant",
            Description = "Search Product Variant",
            OperationId = "product.searchvariants",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<GetProductVariantSearchResponse>> HandleAsync([FromQuery]GetProductVariantSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Read))
                return Unauthorized();
            var response = new GetProductVariantSearchResponse();

            PagingOption<ProductVariantSearchIndex> pagingOption =
                new PagingOption<ProductVariantSearchIndex>(request.CurrentPage, request.SizePerPage);
            
                response.IsDisplayingAll = true;
                
                
                ISearchResponse<ProductVariantSearchIndex> responseElastic;

                if (!request.SearchNameOnly)
                {
                    ElasticSearchHelper<ProductVariantSearchIndex> elasticSearchHelper = new ElasticSearchHelper<ProductVariantSearchIndex>(_elasticClient, request.SearchQuery,
                        ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
                
                    responseElastic = await elasticSearchHelper.GetDocuments();
                }
                else
                {
                    responseElastic =  await _elasticClient.SearchAsync<ProductVariantSearchIndex>
                    (
                        s => s.Size(2000).Index(ElasticIndexConstant.PRODUCT_VARIANT_INDICES).
                            Query(q =>q.
                                QueryString(d =>d.
                                    DefaultField(f =>f.Name).Query("\"" + request.SearchQuery + "\""))));
                }
                // if (request.SearchQuery == null)
                // {
                //     responseElastic = await _elasticClient.SearchAsync<ProductVariantSearchIndex>
                //     (
                //         s => s.Size(2000).Index( ElasticIndexConstant.PRODUCT_VARIANT_INDICES).MatchAll());
                // }
                //
                // else
                // {
                //     responseElastic = await _elasticClient.SearchAsync<ProductVariantSearchIndex>
                //     (
                //         s => s.Size(2000).Index( ElasticIndexConstant.PRODUCT_VARIANT_INDICES).Query(q =>q.QueryString(d =>d.Query('*' + request.SearchQuery + '*'))));
                //
                // }
          
            pagingOption.ResultList = _asyncRepository.ProductVariantIndexFiltering(responseElastic.Documents.ToList(), request,
                new CancellationToken());
            
            pagingOption.ExecuteResourcePaging();
        
            if (!responseElastic.IsValid)
            {
                Console.WriteLine("Invalid Response");
                return Ok(new ApplicationCore.Entities.Products.Product[] { });
            }

            response.Paging = pagingOption;
           
            return Ok(response);
        }
    }
    
      public class SearchProduct : BaseAsyncEndpoint.WithRequest<GetProductSearchRequest>.WithResponse<GetProductSearchResponse>
    {
        private readonly IElasticClient _elasticClient;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;

        public SearchProduct(IElasticClient elasticClient, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository)
        {
            _elasticClient = elasticClient;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
        }

        [HttpGet("api/product/search")]
        [SwaggerOperation(
            Summary = "Search Product",
            Description = "Search Product",
            OperationId = "catalog-items.create",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<GetProductSearchResponse>> HandleAsync([FromQuery]GetProductSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Read))
                return Unauthorized();
            var response = new GetProductSearchResponse();

            PagingOption<ProductSearchIndex> pagingOption =
                new PagingOption<ProductSearchIndex>(request.CurrentPage, request.SizePerPage);
            
                response.IsDisplayingAll = true;
                
                
                ISearchResponse<ProductSearchIndex> responseElastic;

                if (!request.SearchNameOnly)
                {
                    ElasticSearchHelper<ProductSearchIndex> elasticSearchHelper = new ElasticSearchHelper<ProductSearchIndex>(_elasticClient, request.SearchQuery,
                        ElasticIndexConstant.PRODUCT_INDICES);
                
                    responseElastic = await elasticSearchHelper.GetDocuments();
    
                }

                else
                {
                    responseElastic =  await _elasticClient.SearchAsync<ProductSearchIndex>
                    (
                        s => s.Size(2000).Index(ElasticIndexConstant.PRODUCT_INDICES).
                            Query(q =>q.
                                QueryString(d =>d.Fields(f => 
                                    f.Field(p => p.Name)).Query('*' + request.SearchQuery + '*'))));
                }
                
        
            pagingOption.ResultList = _asyncRepository.ProductIndexFiltering(responseElastic.Documents.ToList(), request,
                new CancellationToken());
            
            pagingOption.ExecuteResourcePaging();
        
            if (!responseElastic.IsValid)
            {
                Console.WriteLine("Invalid Response");
                return Ok(new ApplicationCore.Entities.Products.Product[] { });
            }

            response.Paging = pagingOption;
           
            return Ok(response);
        }
    }
      
    public class GetProductBrands : BaseAsyncEndpoint.WithRequest<GetBrandRequest>.WithResponse<GetBrandResponse>
    {
        private readonly IElasticClient _elasticClient;
        private readonly IAuthorizationService _authorizationService;
        private IAsyncRepository<Brand> _brandAsyncRepository;

        public GetProductBrands(IAsyncRepository<Brand> brandAsyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _brandAsyncRepository = brandAsyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }

        [HttpGet("api/product/brands")]
        [SwaggerOperation(
            Summary = "Get Brands of product",
            Description = "Get Brands of product",
            OperationId = "product.getbrands",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<GetBrandResponse>> HandleAsync([FromQuery]GetBrandRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Read))
                return Unauthorized();
            var response = new GetBrandResponse();

            PagingOption<Brand> pagingOption =
                new PagingOption<Brand>(request.CurrentPage, request.SizePerPage);

            response.IsDisplayingAll = true;
            response.Paging = await 
                _brandAsyncRepository.ListAllAsync(pagingOption, cancellationToken);

            return Ok(response);
        }
    }
}