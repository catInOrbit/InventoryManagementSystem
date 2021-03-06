using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Search
{
    public class GetStockTakeOrderById : BaseAsyncEndpoint.WithRequest<STIdRequest>.WithResponse<STIdResponse>
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRedisRepository _redisRepository;

        public GetStockTakeOrderById(IAuthorizationService authorizationService, IAsyncRepository<StockTakeOrder> asyncRepository, IRedisRepository redisRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _redisRepository = redisRepository;
        }
        
        [HttpGet("api/stocktake/{Id}")]
        [SwaggerOperation(
            Summary = "Get all stock take Order or specific with search query",
            Description = "Get all stock take Order or specific with search query",
            OperationId = "st.searchid",
            Tags = new[] { "StockTakingEndpoints" })
        ]
        public override async Task<ActionResult<STIdResponse>> HandleAsync([FromRoute]STIdRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Read))
                return Unauthorized();
            
            var response = new STIdResponse();
            var order = await _asyncRepository.GetByIdAsync(request.Id);
            var stockTakeMessages = await _redisRepository.GetStockTakeAdjustMessage();
            foreach (var stockTakeGroupLocation in order.GroupLocations)
            {
                foreach (var stockTakeItem in stockTakeGroupLocation.CheckItems)
                {
                    stockTakeItem.IsShowingPackageId = true;
                }
            }
            response.IsDisplayingAll = false;
            response.SingleResult = order;
            return Ok(response);
        }
    }
}