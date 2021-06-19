using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Castle.Core.Internal;
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
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderUpdate : BaseAsyncEndpoint.WithRequest<ROUpdateRequest>.WithResponse<ROUpdateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _recevingOrderRepository;
        private readonly IAsyncRepository<PurchaseOrder> _poRepository;
        private readonly IAsyncRepository<Package> _packageRepository;

        private readonly IAsyncRepository<GoodsReceiptOrderSearchIndex> _recevingOrderSearchIndexRepository;

        private readonly IAsyncRepository<ProductVariant> _productRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> recevingOrderRepository, IAsyncRepository<ProductVariant> productRepository, IUserAuthentication userAuthentication, IAsyncRepository<GoodsReceiptOrderSearchIndex> recevingOrderSearchIndexRepository, IAsyncRepository<PurchaseOrder> poRepository, IAsyncRepository<Package> packageRepository)
        {
            _authorizationService = authorizationService;
            _recevingOrderRepository = recevingOrderRepository;
            _productRepository = productRepository;
            _userAuthentication = userAuthentication;
            _recevingOrderSearchIndexRepository = recevingOrderSearchIndexRepository;
            _poRepository = poRepository;
            _packageRepository = packageRepository;
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
                    CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                    TransactionStatus = true
                };

                ro.Transaction = transaction;
            }

            var purhchaseOrder = await _poRepository.GetByIdAsync(request.PurchaseOrderNumber);
            ro.SupplierId = purhchaseOrder.SupplierId;
            ro.PurchaseOrderId = request.PurchaseOrderNumber;
            ro.StorageLocationReceipt = request.StorageLocation;
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

                var package = (await _packageRepository.ListAllAsync(new PagingOption<Package>(0, 0))).ResultList.FirstOrDefault(package => package.ProductVariantId == roi.ProductVariantId);
                
                //Package
                package = new Package
                {
                    ProductVariantId =  roi.ProductVariantId,
                    TotalImportQuantity = roi.QuantityReceived,
                    Location = ro.StorageLocationReceipt,
                    ImportedDate = ro.ReceivedDate,
                    GoodsReceiptOrderId = ro.Id
                };

                await _packageRepository.AddAsync(package);
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
            productVariant.Price = request.SalePrice;
            
            //Update and indexing
            await _productVariantRepository.UpdateAsync(productVariant);
            await _poSearchIndexRepository.ElasticSaveSingleAsync(false,IndexingHelper.ProductSearchIndex(productVariant));
            return Ok();
        }
    }
     
      public class ReceivingOrderSubmit : BaseAsyncEndpoint.WithRequest<ROSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private IAsyncRepository<PurchaseOrder> _poAsyncRepository;
        private IAsyncRepository<PurchaseOrderSearchIndex> _poSearchIndexAsyncRepository;
        private IAsyncRepository<GoodsReceiptOrderSearchIndex> _roSearchIndexAsyncRepository;

        public ReceivingOrderSubmit(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository, IAsyncRepository<PurchaseOrderSearchIndex> poSearchIndexAsyncRepository, IAsyncRepository<GoodsReceiptOrderSearchIndex> roSearchIndexAsyncRepository)
        {
            _roAsyncRepository = roAsyncRepository;
            _poAsyncRepository = poAsyncRepository;
            _poSearchIndexAsyncRepository = poSearchIndexAsyncRepository;
            _roSearchIndexAsyncRepository = roSearchIndexAsyncRepository;
        }

        [HttpPost("api/goodsreceipt/submit")]
        [SwaggerOperation(
            Summary = "Submit Goods Receipt Order",
            Description = "Submit Goods Receipt Order",
            OperationId = "gr.submit",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult> HandleAsync(ROSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var ro = await _roAsyncRepository.GetByIdAsync(request.ReceivingOrderId);
            var po = await _poAsyncRepository.GetByIdAsync(ro.PurchaseOrderId);
            ro.Transaction.ModifiedDate = DateTime.Now;

            var response = new ROSubmitResponse();
            foreach (var goodsReceiptOrderItem in ro.ReceivedOrderItems)
            {
                if (goodsReceiptOrderItem.QuantityReceived < po.PurchaseOrderProduct
                    .FirstOrDefault(orderItem => orderItem.ProductVariantId == goodsReceiptOrderItem.ProductVariantId)
                    .OrderQuantity)
                {
                    response.IncompletePurchaseOrderId = po.Id;
                    response.IncompleteVariantId.Add(goodsReceiptOrderItem.ProductVariantId);
                }
            }
            
            if(response.IncompleteVariantId.IsNullOrEmpty())
                po.PurchaseOrderStatus = PurchaseOrderStatusType.Done;
            
            await _poAsyncRepository.UpdateAsync(po);
            await _roAsyncRepository.UpdateAsync(ro);
            await _poSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po));
            await _roSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.GoodsReceiptOrderSearchIndex(ro));
            
            if(!response.IncompleteVariantId.IsNullOrEmpty())
                return Ok(response);
            return Ok();
        }
    }
    
    
}