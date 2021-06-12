using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderConfirm : BaseAsyncEndpoint.WithRequest<POConfirmRequest>.WithoutResponse
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IAuthorizationService _authorizationService;

        public PurchaseOrderConfirm(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService)
        {
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
        }

        [HttpPost("api/po/confirm/{PurchaseOrderNumber}")]
        [SwaggerOperation(
            Summary = "Confirm purchase order ",
            Description = "Confirm purchase order",
            OperationId = "catalog-items.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(POConfirmRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Update))
                return Unauthorized();
            
            var po = _purchaseOrderRepos.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            po.PurchaseOrderStatus = PurchaseOrderStatusType.POConfirm;
            await _purchaseOrderRepos.UpdateAsync(po);
            return Ok();
        }
    }
}