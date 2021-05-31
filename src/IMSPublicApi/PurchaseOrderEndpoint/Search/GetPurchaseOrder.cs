using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetPurchaseOrder : BaseAsyncEndpoint.WithoutRequest.WithResponse<GetPurchaseOrderResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetPurchaseOrder(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/purchaseorder")]
        [SwaggerOperation(
            Summary = "Get all purchase Order",
            Description = "Get all purchase Order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<GetPurchaseOrderResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetPurchaseOrderResponse();
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Read);
            
            if (!isAuthorized.Succeeded)
                return Unauthorized();
            
            var pos = await _asyncRepository.ListAllAsync(cancellationToken);
            response.PurchaseOrders = pos.ToList();
            return Ok(response);
        }
    }
}