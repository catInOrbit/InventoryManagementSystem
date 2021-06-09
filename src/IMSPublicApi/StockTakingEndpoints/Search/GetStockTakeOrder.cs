using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Search
{
    public class GetStockTakeOrder : BaseAsyncEndpoint.WithRequest<STSearchRequest>.WithResponse<STSearchResponse>
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public GetStockTakeOrder(IAuthorizationService authorizationService, IElasticClient elasticClient, IAsyncRepository<StockTakeOrder> asyncRepository)
        {
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
            _asyncRepository = asyncRepository;
        }

        [HttpGet("api/stocktake/search/{SearchQuery}")]
        [SwaggerOperation(
            Summary = "Get all stock take Order or specific with search query",
            Description = "Get all stock take Order or specific with search query",
            OperationId = "st.search",
            Tags = new[] { "StockTakingEndpoints" })
        ]

        public override async Task<ActionResult<STSearchResponse>> HandleAsync([FromRoute] STSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "StockTakeOrder", UserOperations.Read))
                return Unauthorized();

            var response = new STSearchResponse();
            if (request.SearchQuery == "all")
            {
                response.IsDisplayingAll = true;
                response.StockTakeSearchIndices = (await _asyncRepository.GetSTForELIndexAsync(cancellationToken)).ToList();
                return Ok(response);
            }
            else
            {
                var pos = await _asyncRepository.ListAllAsync(cancellationToken);
                var responseElastic = await _elasticClient.SearchAsync<StockTakeSearchIndex>(
                    s => s.Index("stocktakeorders").Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
                
                return Ok(responseElastic.Documents);
            }
        }
    }
}