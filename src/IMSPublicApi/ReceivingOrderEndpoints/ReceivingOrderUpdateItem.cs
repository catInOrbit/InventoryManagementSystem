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

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderUpdateItem : BaseAsyncEndpoint.WithRequest<ROUpdateItemRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _recevingOrderRepository;
        private readonly IAsyncRepository<ProductVariant> _productRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderUpdateItem(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> recevingOrderRepository, IAsyncRepository<ProductVariant> productRepository, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _recevingOrderRepository = recevingOrderRepository;
            _productRepository = productRepository;
            _userAuthentication = userAuthentication;
        }

        
        [HttpPost("api/receiving/adjust")]
        [SwaggerOperation(
            Summary = "Update Receiving Order",
            Description = "Update Receiving Order",
            OperationId = "ro.update",
            Tags = new[] { "ReceivingOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ROUpdateItemRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();

            var ro = await _recevingOrderRepository.GetByIdAsync(request.CurrentReceivingOrderId);
            var productVariant = await _productRepository.GetByIdAsync(request.ProductVariantId);
            ro.Transaction.ModifiedDate = DateTime.Now;
            ro.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;

            foreach (var roReceivedOrderItem in ro.ReceivedOrderItems)
            {
                if (roReceivedOrderItem.Id == request.ProductVariantId)
                {
                    productVariant.Unit = request.UnitUpdate;
                    productVariant.Quantity = request.QuantityUpdate;
                }
            }

            await _recevingOrderRepository.UpdateAsync(ro);
            await _recevingOrderRepository.ElasticSaveSingleAsync(ro);
            return Ok();
        }
    }
}