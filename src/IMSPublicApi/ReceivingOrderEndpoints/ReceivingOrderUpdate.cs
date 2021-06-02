using System;
using System.Linq;
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
    public class ReceivingOrderUpdate : BaseAsyncEndpoint.WithRequest<ROEditRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ReceivingOrder> _receivingOrderRepository;
        private readonly IAsyncRepository<PurchaseOrder> _purchaseOrderRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;
        
        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<ReceivingOrder> receivingOrderRepository, IAsyncRepository<PurchaseOrder> purchaseOrderRepository, IAsyncRepository<ProductVariant> productAsyncRepository)
        {
            _authorizationService = authorizationService;
            _receivingOrderRepository = receivingOrderRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _productVariantRepository = productAsyncRepository;
        }

        [HttpPost("api/receiving/update")]
        [SwaggerOperation(
            Summary = "Create G",
            Description = "Authenticates a user",
            OperationId = "auth.authenticate",
            Tags = new[] { "IMSAuthenticationEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ROEditRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            var ro = await _receivingOrderRepository.GetByIdAsync(request.ReceiveOrderGet);
            ro.ModifiedDate = DateTime.Now;
            ro.PurchaseOrderId = request.PurchaseOrderNumber;
            var po = _purchaseOrderRepository.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            if (po != null)
            {
                foreach (var purchaseOrderItem in po.PurchaseOrderProduct)
                {
                    var roi = new ReceivedOrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductVariant = purchaseOrderItem.ProductVariant,
                        Quantity =  purchaseOrderItem.Quantity,
                        ProductVariantId = purchaseOrderItem.ProductVariantId,
                        StorageLocation = request.StorageLocation,
                        ReceivedOrderId = ro.Id,
                        QuantityInventory = (await _productVariantRepository.GetByIdAsync(purchaseOrderItem.ProductVariantId)).Quantity
                    };
                    ro.ReceivedOrderItems.Add(roi);
                }
            }

            await _receivingOrderRepository.UpdateAsync(ro);
            return Ok();
        }
    }
}