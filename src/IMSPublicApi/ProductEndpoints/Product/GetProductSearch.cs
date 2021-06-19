using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProduct : BaseAsyncEndpoint.WithRequest<GetProductAllRequest>.WithResponse<GetProductSearchResponse>
    {
        private readonly IElasticClient _elasticClient;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;

        public GetProduct(IElasticClient elasticClient, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository)
        {
            _elasticClient = elasticClient;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
        }

        [HttpPost("api/product/all/")]
        [SwaggerOperation(
            Summary = "Search Product by Name",
            Description = "Search Product by Id",
            OperationId = "catalog-items.create",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<GetProductSearchResponse>> HandleAsync(GetProductAllRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
            //     return Unauthorized();
            var response = new GetProductSearchResponse();

            PagingOption<ProductSearchIndex> pagingOption =
                new PagingOption<ProductSearchIndex>(request.CurrentPage, request.SizePerPage);
            
                response.IsDisplayingAll = true;
                response.Paging = await 
                    _asyncRepository.GetProductForELIndexAsync(pagingOption, cancellationToken);

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

        [HttpGet("api/product/search/{Query}&page={CurrentPage}&size={SizePerPage}")]
        [SwaggerOperation(
            Summary = "Search Product by Name",
            Description = "Search Product by Id",
            OperationId = "catalog-items.create",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<GetProductSearchResponse>> HandleAsync([FromRoute]GetProductSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            var response = new GetProductSearchResponse();

            PagingOption<ProductSearchIndex> pagingOption =
                new PagingOption<ProductSearchIndex>(request.CurrentPage, request.SizePerPage);
            
                response.IsDisplayingAll = true;

            var responseElastic = await _elasticClient.SearchAsync<ProductSearchIndex>
            (
                s => s.Index( ElasticIndexConstant.PRODUCT_INDICES).Query(q =>q.QueryString(d =>d.Query('*' + request.Query + '*'))));
        
            foreach (var productSearchIndex in responseElastic.Documents)
                pagingOption.ResultList.Add(productSearchIndex);
        
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
}