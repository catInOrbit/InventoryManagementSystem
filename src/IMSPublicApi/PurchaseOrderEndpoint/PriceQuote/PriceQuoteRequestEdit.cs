using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteRequestEdit : BaseAsyncEndpoint.WithRequest<PriceQuoteRequestEditRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<PriceQuoteOrder> _asyncRepository;

        public PriceQuoteRequestEdit(IAuthorizationService authorizationService, IAsyncRepository<PriceQuoteOrder> asyncRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
        }
        
        
        [HttpPut("api/pricequote/edit")]
        [SwaggerOperation(
            Summary = "Edit price quote request",
            Description = "Edit price quote request",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(PriceQuoteRequestEditRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PriceQuoteOrder",
                UserOperations.Create);
            
            if (!isAuthorized.Succeeded)
                return Unauthorized();
            
            await _asyncRepository.UpdateAsync(request.PriceQuoteOrder);
            return Ok();
        }
    }
}