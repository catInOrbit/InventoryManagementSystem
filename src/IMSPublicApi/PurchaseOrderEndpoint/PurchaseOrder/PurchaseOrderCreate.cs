using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
     
    public class PurchaseOrderCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<PurchaseOrderCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IUserAuthentication _userAuthentication;

        public PurchaseOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos)
        {
            _authorizationService = authorizationService;
            _purchaseOrderRepos = purchaseOrderRepos;
        }

        [HttpPost("api/purchaseorder/create")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "po.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<PurchaseOrderCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new PurchaseOrderCreateResponse();

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Create);

            var purchaseOrder = new ApplicationCore.Entities.Orders.PurchaseOrder();
            purchaseOrder.CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            purchaseOrder.SupplierId = null;

            response.PurchaseOrder = purchaseOrder;
            await _purchaseOrderRepos.AddAsync(purchaseOrder);

            return Ok(response);
        }
    }
}