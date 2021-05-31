using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
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

        [HttpPost("api/product/search")]
        [SwaggerOperation(
            Summary = "Search Product by Id",
            Description = "Search Product by Id",
            OperationId = "catalog-items.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] GetProductSearchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "Product",
                UserOperations.Read);
            
            if (!isAuthorized.Succeeded)
                return Unauthorized();
            
            var page = 1;
            var pageSize = 5;
            // var response = await _elasticClient.SearchAsync<ProductIndex>(
            //     s => s.Query(q => q.QueryString(d => d.Query('*' + request.Query + '*'))));
            var response = await _elasticClient.SearchAsync<Product>(
                s => s.Query(q =>  q.Match(m => m.Field(f => f.Name).Query(request.Query))));

            
            if (!response.IsValid)
            {
                Console.WriteLine("Invalid Response");
                return Ok(new Product[] { });
            }

            return Ok(response.Documents);
        }
    }
}