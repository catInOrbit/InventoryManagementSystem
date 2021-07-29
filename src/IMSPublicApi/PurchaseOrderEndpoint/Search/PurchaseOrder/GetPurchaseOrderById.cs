using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
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
            OperationId = "po.searchid",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync([FromRoute] GetPurchaseOrderIdRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Read))
                return Unauthorized();
            
            var response = new GetAllPurchaseOrderResponse();
            response.IsDisplayingAll = false;

            response.PurchaseOrder = await _purchaseAsyncRepository.GetByIdAsync(request.Id);
            response.PurchaseOrder.PurchaseOrderStatusString = response.PurchaseOrder.PurchaseOrderStatus.ToString();
            foreach (var orderItem in response.PurchaseOrder.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;

            response.PurchaseOrder.PurchaseOrderStatusString = response.PurchaseOrder.PurchaseOrderStatus.ToString();
            response.MergedOrderIdLists =
                _purchaseAsyncRepository.GetMergedPurchaseOrders(response.PurchaseOrder.Id);
            return Ok(response);
        }
    }
}