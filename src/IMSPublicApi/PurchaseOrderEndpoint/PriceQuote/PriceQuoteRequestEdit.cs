using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
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
        private readonly IAsyncRepository<ProductVariant> _productVariantRepos;

        public PriceQuoteRequestEdit(IAuthorizationService authorizationService, IAsyncRepository<PriceQuoteOrder> asyncRepository, IUserAuthentication userAuthentication, IAsyncRepository<ProductVariant> productVariantRepos)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productVariantRepos = productVariantRepos;
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
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PriceQuoteOrder", UserOperations.Create))
                return Unauthorized();

            var pqr = _asyncRepository.GetPriceQuoteByNumber(request.PriceQuoteNumberGet);
            pqr.Transaction.ModifiedDate = DateTime.Now;
            pqr.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
            {
                requestOrderItemInfo.OrderNumber = pqr.PriceQuoteNumber;
                requestOrderItemInfo.ProductVariant = await _productVariantRepos.GetByIdAsync(requestOrderItemInfo.ProductVariantId);
                requestOrderItemInfo.TotalAmount += requestOrderItemInfo.Price;
                pqr.TotalOrderAmount += requestOrderItemInfo.Price;
                pqr.PurchaseOrderProduct.Add(requestOrderItemInfo);
            }
            
            pqr.MailDescription = request.MailDescription;
            pqr.SupplierId = request.SupplierId;
            pqr.Deadline = request.Deadline;
            
            await _asyncRepository.UpdateAsync(pqr);

            var response = new PQEditResponse();
            response.PriceQuoteResponse = pqr;
            return Ok(response);
        }
    }
}