using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
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
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _poSearchRepos;

        private readonly IAuthorizationService _authorizationService;

        public PurchaseOrderConfirm(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService, IAsyncRepository<PurchaseOrderSearchIndex> poSearchRepos)
        {
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
            _poSearchRepos = poSearchRepos;
        }

        [HttpPost("api/po/confirm/{PurchaseOrderNumber}")]
        [SwaggerOperation(
            Summary = "Confirm purchase order ",
            Description = "Confirm purchase order",
            OperationId = "catalog-items.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute]POConfirmRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Update))
                return Unauthorized();
            
            var po = _purchaseOrderRepos.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            po.PurchaseOrderStatus = PurchaseOrderStatusType.POConfirm;
            await _purchaseOrderRepos.UpdateAsync(po);
            await _poSearchRepos.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

            return Ok();
        }
    }
}