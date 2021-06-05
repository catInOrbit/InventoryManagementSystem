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

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetPurchaseOrderById : BaseAsyncEndpoint.WithRequest<GetAllPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
    {
        private IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseAsyncRepository;
        private IAuthorizationService _authorizationService;
        public GetPurchaseOrderById(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseAsyncRepository, IAuthorizationService authorizationService)
        {
            _purchaseAsyncRepository = purchaseAsyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/purchaseorder/number/{SearchQuery}")]
        [SwaggerOperation(
            Summary = "Get all purchase Order",
            Description = "Get all purchase Order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync(GetAllPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
                return Unauthorized();
            
            var response = new GetAllPurchaseOrderResponse();
            response.IsDisplayingAll = false;

            response.PurchaseOrder = _purchaseAsyncRepository.GetPurchaseOrderByNumber(request.SearchQuery);

            return Ok(response);
        }
    }
}