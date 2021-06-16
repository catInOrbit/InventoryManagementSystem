using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderUpdate : BaseAsyncEndpoint.WithRequest<ROUpdateRequest>.WithResponse<ROUpdateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _recevingOrderRepository;
        private readonly IAsyncRepository<PurchaseOrder> _poRepository;

        private readonly IAsyncRepository<GoodsReceiptOrderSearchIndex> _recevingOrderSearchIndexRepository;

        private readonly IAsyncRepository<ProductVariant> _productRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> recevingOrderRepository, IAsyncRepository<ProductVariant> productRepository, IUserAuthentication userAuthentication, IAsyncRepository<GoodsReceiptOrderSearchIndex> recevingOrderSearchIndexRepository, IAsyncRepository<PurchaseOrder> poRepository)
        {
            _authorizationService = authorizationService;
            _recevingOrderRepository = recevingOrderRepository;
            _productRepository = productRepository;
            _userAuthentication = userAuthentication;
            _recevingOrderSearchIndexRepository = recevingOrderSearchIndexRepository;
            _poRepository = poRepository;
        }
        
        [HttpPut("api/goodsreceipt/update")]
        [SwaggerOperation(
            Summary = "Update Receiving Order",
            Description = "Update Receiving Order",
            OperationId = "gr.updateitem",
            Tags = new[] { "GoodsReceiptOrders" })
        ]

        public override async Task<ActionResult<ROUpdateResponse>> HandleAsync(ROUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Read))
                return Unauthorized();
            
            var ro = new GoodsReceiptOrder();
            
            //Check if creating for first time for elastic and addasync procedure

            //Create new transaction if null
            if (ro.Transaction == null)
            {
                var transaction = new Transaction
                {
                    CreatedDate = DateTime.Now,
                    Type = TransactionType.Purchase,
                    CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
                };

                ro.Transaction = transaction;
            }

            var purhchaseOrder = await _poRepository.GetByIdAsync(request.PurchaseOrderNumber);
            ro.SupplierId = purhchaseOrder.SupplierId;
            ro.PurchaseOrderId = request.PurchaseOrderNumber;
            
            //TODO: Update sku of product in a seperate API
            
            //Data of receipt order is from frontend

            ro.ReceivedOrderItems.Clear();
            foreach (var item in request.UpdateItems)
            {
                var roi = new GoodsReceiptOrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    QuantityReceived =  item.QuantityReceived,
                    ProductVariantId = item.ProductVariantId,
                    GoodsReceiptOrderId = ro.Id,
                    ProductVariantName = (await _productRepository.GetByIdAsync(item.ProductVariantId)).Name
                };
                ro.ReceivedOrderItems.Add(roi);
            }

            //Update and indexing
            await _recevingOrderRepository.AddAsync(ro);
            await _recevingOrderSearchIndexRepository.ElasticSaveSingleAsync(true,IndexingHelper.GoodsReceiptOrderSearchIndex(ro));

            var response = new ROUpdateResponse();
            response.CreatedGoodsReceiptId = ro.Id;
            return Ok(response);
        }
    }
    
     public class ReceivingOrderUpdateProductItem : BaseAsyncEndpoint.WithRequest<ROSingleProductUpdateRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ProductSearchIndex> _poSearchIndexRepository;

        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderUpdateProductItem(IAuthorizationService authorizationService, IAsyncRepository<ProductSearchIndex> poSearchIndexRepository, IAsyncRepository<ProductVariant> productVariantRepository, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _poSearchIndexRepository = poSearchIndexRepository;
            _productVariantRepository = productVariantRepository;
            _userAuthentication = userAuthentication;
        }


        [HttpPut("api/goodsreceipt/updateitem")]
        [SwaggerOperation(
            Summary = "Update Receiving Order",
            Description = "Update Receiving Order",
            OperationId = "gr.updateitem",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult> HandleAsync(ROSingleProductUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
        
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Read))
                return Unauthorized();
            
            //Get Product Variant
            var productVariant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId);
            
            //Update SKU, LOCATION, PRICE
            productVariant.Sku = request.Sku;
            productVariant.StorageLocation = request.ProductStorageLocation;
            productVariant.Price = request.SalePrice;
            
            //Update and indexing
            await _productVariantRepository.UpdateAsync(productVariant);
            await _poSearchIndexRepository.ElasticSaveSingleAsync(false,IndexingHelper.ProductSearchIndex(productVariant));
            return Ok();
        }
    }
    
    
}