using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderDelete : BaseAsyncEndpoint.WithRequest<PurchaseOrderDeleteRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;

        public PurchaseOrderDelete(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
        }
        
        [HttpDelete("api/purchaseorder/delete/{Id}")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "po.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] PurchaseOrderDeleteRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO: IMPORTANT: Database relationship for child table must have cascade in insert and delete option
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Delete);
            
            if (!isAuthorized.Succeeded)
                return Unauthorized();

            var po = await _asyncRepository.GetByIdAsync(request.Id);
           await _asyncRepository.DeleteAsync(po,
               cancellationToken);
           return Ok();
        }
    }
}