using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetPurchaseOrder : BaseAsyncEndpoint.WithRequest<GetPurchaseOrderRequest>.WithResponse<GetPurchaseOrderResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetPurchaseOrder(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/purchaseorder/{number}")]
        [SwaggerOperation(
            Summary = "Get all purchase Order",
            Description = "Get all purchase Order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<GetPurchaseOrderResponse>> HandleAsync(GetPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetPurchaseOrderResponse();
            // var isAuthorized = await _authorizationService.AuthorizeAsync(
            //     HttpContext.User, "PurchaseOrder",
            //     UserOperations.Read);
            //
            // if (!isAuthorized.Succeeded)
            //     return Unauthorized();
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
                return Unauthorized();

            if (request.number == "all")
            {
                var pos = await _asyncRepository.ListAllAsync(cancellationToken);
                response.PurchaseOrders = pos.ToList();
            }

            else
            {
                var po = _asyncRepository.GetPurchaseOrderByNumber(request.number, cancellationToken);
                response.PurchaseOrders.Add(po);
            }
            return Ok(response);
        }
    }
}