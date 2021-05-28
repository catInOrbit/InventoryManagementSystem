using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetProductSearch : BaseAsyncEndpoint.WithRequest<GetProductSearchRequest>.WithoutResponse
    {
        private readonly IElasticClient _elasticClient;

        public GetProductSearch(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        [HttpPost("api/productsearch/{Query}")]
        [SwaggerOperation(
            Summary = "Search",
            Description = "Creates a new Catalog Item",
            OperationId = "catalog-items.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] GetProductSearchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var page = 1;
            var pageSize = 5;
            var response = await _elasticClient.SearchAsync<Product>(
                s => s.Query(q => q.QueryString(d => d.Query('*' + request.Query + '*'))));
            
            
 

            if (!response.IsValid)
            {
                Console.WriteLine("Invalid Response");
                return Ok(new Product[] { });
            }

            return Ok(response.Documents);
        }
    }
}