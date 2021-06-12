using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
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
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepos;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        public PriceQuoteRequestEdit(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserAuthentication userAuthentication, IAsyncRepository<ProductVariant> productVariantRepos)
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

            var po = _asyncRepository.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
            {
                requestOrderItemInfo.OrderNumber = po.PurchaseOrderNumber;
                requestOrderItemInfo.ProductVariant = await _productVariantRepos.GetByIdAsync(requestOrderItemInfo.ProductVariantId);
                requestOrderItemInfo.TotalAmount += requestOrderItemInfo.Price;
                po.TotalOrderAmount += requestOrderItemInfo.Price;
                po.PurchaseOrderProduct.Add(requestOrderItemInfo);
            }
            
            po.MailDescription = request.MailDescription;
            po.SupplierId = request.SupplierId;
            po.Deadline = request.Deadline;
            
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(IndexingHelper.PurchaseOrderSearchIndex(po));

            var response = new PQEditResponse();
            response.PriceQuoteResponse = po;
            return Ok(response);
        }
    }
}