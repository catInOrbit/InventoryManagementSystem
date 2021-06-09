using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Search
{
    public class GetStockTakeOrderById : BaseAsyncEndpoint.WithRequest<STIdRequest>.WithResponse<STSearchRequest>
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetStockTakeOrderById(IAuthorizationService authorizationService, IAsyncRepository<StockTakeOrder> asyncRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
        }
        
        [HttpGet("api/stocktake/{Id}")]
        [SwaggerOperation(
            Summary = "Get all stock take Order or specific with search query",
            Description = "Get all stock take Order or specific with search query",
            OperationId = "st.search",
            Tags = new[] { "StockTakingEndpoints" })
        ]
        public override async Task<ActionResult<STSearchRequest>> HandleAsync([FromRoute]STIdRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "StockTakeOrder", UserOperations.Read))
                return Unauthorized();
            
            var response = new STSearchResponse();
            var order = await _asyncRepository.GetByIdAsync(request.Id);
            response.IsDisplayingAll = false;
            response.StockTakeOrder = order;

            return Ok(response);
        }
    }
}