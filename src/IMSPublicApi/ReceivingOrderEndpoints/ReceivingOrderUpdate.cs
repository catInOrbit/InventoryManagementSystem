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
        private readonly IAsyncRepository<GoodsReceiptOrder> _receivingOrderRepository;
        private readonly IAsyncRepository<PurchaseOrder> _purchaseOrderRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> receivingOrderRepository, IAsyncRepository<PurchaseOrder> purchaseOrderRepository, IAsyncRepository<ProductVariant> productAsyncRepository, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _receivingOrderRepository = receivingOrderRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _productVariantRepository = productAsyncRepository;
            _userAuthentication = userAuthentication;
        }

        [HttpPost("api/receiving/update")]
        [SwaggerOperation(
            Summary = "Create Receiving Order",
            Description = "Create Receiving Order",
            OperationId = "ro.create",
            Tags = new[] { "ReceivingOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ROEditRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            var ro = await _receivingOrderRepository.GetByIdAsync(request.ReceiveOrderGet);
            ro.Transaction.ModifiedDate = DateTime.Now;
            ro.PurchaseOrderId = request.PurchaseOrderNumber;
            ro.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            var po = _purchaseOrderRepository.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            if (po != null)
            {
                foreach (var purchaseOrderItem in po.PurchaseOrderProduct)
                {
                    var roi = new GoodsReceiptOrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductVariant = purchaseOrderItem.ProductVariant,
                        Quantity =  purchaseOrderItem.OrderQuantity,
                        ProductVariantId = purchaseOrderItem.ProductVariantId,
                        StorageLocation = request.StorageLocation,
                        ReceivedOrderId = ro.Id,
                        QuantityInventory = (await _productVariantRepository.GetByIdAsync(purchaseOrderItem.ProductVariantId)).StorageQuantity
                    };
                    ro.ReceivedOrderItems.Add(roi);
                }
            }

            await _receivingOrderRepository.UpdateAsync(ro);
            await _receivingOrderRepository.ElasticSaveSingleAsync(ro);
            return Ok();
        }
    }
}