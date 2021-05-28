using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    [Authorize]
    public class PurchaseOrderGet : BaseAsyncEndpoint.WithoutRequest.WithResponse<PurchaseOrderGetResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseAsyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public PurchaseOrderGet(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseAsyncRepository, IAuthorizationService authorizationService)
        {
            _purchaseAsyncRepository = purchaseAsyncRepository;
            _authorizationService = authorizationService;
        }

        public override async Task<ActionResult<PurchaseOrderGetResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new PurchaseOrderGetResponse();
            
            var purchaseOrders = await _purchaseAsyncRepository.ListAllAsync();
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Read);

            if (!isAuthorized.Succeeded)
            {
                response.Result = false;
                response.Verbose = "Not authorized as privileged user";
                return Unauthorized(response);
            }
            response.PurchaseOrders = purchaseOrders.ToList();

            return Ok(response);
        }
    }
}