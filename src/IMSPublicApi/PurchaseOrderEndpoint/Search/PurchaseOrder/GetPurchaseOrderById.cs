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
    public class GetPurchaseOrderById : BaseAsyncEndpoint.WithRequest<GetPurchaseOrderIdRequest>.WithResponse<GetAllPurchaseOrderResponse>
    {
        private IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseAsyncRepository;
        private IAuthorizationService _authorizationService;
        public GetPurchaseOrderById(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseAsyncRepository, IAuthorizationService authorizationService)
        {
            _purchaseAsyncRepository = purchaseAsyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/purchaseorder/number/{Id}")]
        [SwaggerOperation(
            Summary = "Get a purchase Order",
            Description = "Get a purchase Order",
            OperationId = "po.searchnumber",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync([FromRoute] GetPurchaseOrderIdRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
                return Unauthorized();
            
            var response = new GetAllPurchaseOrderResponse();
            response.IsDisplayingAll = false;

            response.PurchaseOrder = await _purchaseAsyncRepository.GetByIdAsync(request.Id);

            return Ok(response);
        }
    }
}