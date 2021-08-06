using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetProductUpdateMessages : BaseAsyncEndpoint.WithoutRequest.WithResponse<ProductUpdateMessageResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IRedisRepository _redisRepository;

        public GetProductUpdateMessages(IAuthorizationService authorizationService, IRedisRepository redisRepository)
        {
            _authorizationService = authorizationService;
            _redisRepository = redisRepository;
        }

        [HttpGet("api/product/updatemessage")]
        [SwaggerOperation(
            Summary = "Get list of products with updated request",
            Description = "Get list of products with updated request",
            OperationId = "product.updatemessage",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<ProductUpdateMessageResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT_SEARCH, UserOperations.Update))
                return Unauthorized();
            var response = new ProductUpdateMessageResponse();
            response.ProductUpdateMessages =  await _redisRepository.GetProductUpdateMessage();
            return Ok(response);
        }
    }
}