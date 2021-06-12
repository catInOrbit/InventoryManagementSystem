using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProductSearch : BaseAsyncEndpoint.WithRequest<GetProductSearchRequest>.WithoutResponse
    {
        private readonly IElasticClient _elasticClient;
        private readonly IAuthorizationService _authorizationService;
        public GetProductSearch(IElasticClient elasticClient, IAuthorizationService authorizationService)
        {
            _elasticClient = elasticClient;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/product/search/{Query}&page={CurrentPage}&size={SizePerPage}")]
        [SwaggerOperation(
            Summary = "Search Product by Name",
            Description = "Search Product by Id",
            OperationId = "catalog-items.create",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] GetProductSearchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            // var response = await _elasticClient.SearchAsync<ProductSearchIndex>(
            //     s => s.Query(q => q.QueryString(d => d.Query('*' + request.Query + '*'))));
            // var response = await _elasticClient.SearchAsync<ProductIndex>(
            //     s => s.Query(q =>  q.Match(m => m.Field(f => f.Name).Query(request.Query))));
            //
            
            var response = await _elasticClient.SearchAsync<ProductSearchIndex>
            (
                s => s.From(request.CurrentPage).Size(request.SizePerPage).Index("productindices").Query(q =>q.QueryString(d =>d.Query('*' + request.Query + '*'))));
            
            if (!response.IsValid)
            {
                Console.WriteLine("Invalid Response");
                return Ok(new ApplicationCore.Entities.Products.Product[] { });
            }

            return Ok(response.Documents);
        }
    }
}