using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderUpdate : BaseAsyncEndpoint.WithRequest<ROEditRequest>.WithResponse<ROCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _receivingOrderRepository;
        private readonly IAsyncRepository<GoodsReceiptOrderSearchIndex> _receivingOrderSearchRepository;

        private readonly IAsyncRepository<PurchaseOrder> _purchaseOrderRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> receivingOrderRepository, IAsyncRepository<PurchaseOrder> purchaseOrderRepository, IAsyncRepository<ProductVariant> productAsyncRepository, IUserAuthentication userAuthentication, IAsyncRepository<GoodsReceiptOrderSearchIndex> receivingOrderSearchRepository)
        {
            _authorizationService = authorizationService;
            _receivingOrderRepository = receivingOrderRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _productVariantRepository = productAsyncRepository;
            _userAuthentication = userAuthentication;
            _receivingOrderSearchRepository = receivingOrderSearchRepository;
        }

        [HttpPut("api/goodsreceipt/update")]
        [SwaggerOperation(
            Summary = "Update good receipt order",
            Description = "Update good receipt order",
            OperationId = "gr.update",
            Tags = new[] { "GoodsReceiptOrders" })
        ]

        public override async Task<ActionResult<ROCreateResponse>> HandleAsync(ROEditRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
             if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();

             var response = new ROCreateResponse();
            var ro =  _receivingOrderRepository.GetReceivingOrderByNumber(request.ReceiveOrderNumber);
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
                        QuantityReceived =  purchaseOrderItem.OrderQuantity,
                        ProductVariantId = purchaseOrderItem.ProductVariantId,
                        GoodsReceiptOrderId = ro.Id,
                        ProductStorageLocation = purchaseOrderItem.ProductVariant.StorageLocation
                    };
                    ro.ReceivedOrderItems.Add(roi);
                }
            }
            ro.SupplierId = po.SupplierId;
            await _receivingOrderRepository.UpdateAsync(ro);
            await _receivingOrderSearchRepository.ElasticSaveSingleAsync(false,IndexingHelper.GoodsReceiptOrderSearchIndex(ro));
            response.ReceivingOrder = ro;
            return Ok(response);
        }
    }
}