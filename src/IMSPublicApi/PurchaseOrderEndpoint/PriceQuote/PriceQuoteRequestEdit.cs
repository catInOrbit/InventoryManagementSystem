using System;
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

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteRequestEdit : BaseAsyncEndpoint.WithRequest<PQEditRequest>.WithResponse<PQEditResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<PriceQuoteOrder> _asyncRepository;
        private readonly IUserAuthentication _userAuthentication;

        public PriceQuoteRequestEdit(IAuthorizationService authorizationService, IAsyncRepository<PriceQuoteOrder> asyncRepository, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
        }
        
        
        [HttpPut("api/pricequote/edit")]
        [SwaggerOperation(
            Summary = "Edit price quote request",
            Description = "Edit price quote request",
            OperationId = "po.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]
        public override async Task<ActionResult<PQEditResponse>> HandleAsync(PQEditRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PriceQuoteOrder",
                UserOperations.Create);
            
            if (!isAuthorized.Succeeded)
                return Unauthorized();

            var pqr = _asyncRepository.GetPriceQuoteByNumber(request.PriceQuoteNumberGet);
            pqr.ModifiedDate = DateTime.Now;
            pqr.ModifiedBy = (await _userAuthentication.GetCurrentSessionUser()).Id;
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
                pqr.PurchaseOrderProduct.Add(requestOrderItemInfo);
            pqr.SupplierId = request.SupplierId;
            pqr.Deadline = request.Deadline;
            
            await _asyncRepository.UpdateAsync(pqr);

            var response = new PQEditResponse();
            response.PriceQuoteResponse = pqr;
            return Ok(response);
        }
    }
}