using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetPriceQuote : BaseAsyncEndpoint.WithoutRequest.WithResponse<GetPriceQuoteResponse>
    {
        private IAsyncRepository<PriceQuoteOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;


        public GetPriceQuote(IAsyncRepository<PriceQuoteOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet("api/pricequote")]
        [SwaggerOperation(
            Summary = "Get all price quote",
            Description = "Get all price quote",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<GetPriceQuoteResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Read);
            
            if (!isAuthorized.Succeeded)
                return Unauthorized();
            
            var response = new GetPriceQuoteResponse();
            var pqrs = await _asyncRepository.ListAllAsync(cancellationToken);
            response.PriceQuoteOrders = pqrs.ToList();
            return Ok(response);
        }
    }
}