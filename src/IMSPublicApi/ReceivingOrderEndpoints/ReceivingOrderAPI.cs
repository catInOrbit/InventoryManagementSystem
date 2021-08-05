using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Castle.Core.Internal;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.RedisMessages;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderUpdate : BaseAsyncEndpoint.WithRequest<ROUpdateRequest>.WithResponse<ROUpdateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _recevingOrderRepository;
        private readonly IAsyncRepository<PurchaseOrder> _poRepository;
        private readonly IAsyncRepository<Package> _packageRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;

        private readonly IElasticAsyncRepository<ProductVariantSearchIndex> _productVariantElasticRepository;

        private readonly IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> _recevingOrderSearchIndexRepository;

        private readonly IUserSession _userAuthentication;
        private readonly IRedisRepository _redisRepository;

        private readonly INotificationService _notificationService;

        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> recevingOrderRepository, IUserSession userAuthentication,  IAsyncRepository<PurchaseOrder> poRepository, IAsyncRepository<Package> packageRepository, INotificationService notificationService, IAsyncRepository<Location> locationRepository, IAsyncRepository<ProductVariant> productVariantRepository, IElasticAsyncRepository<ProductVariantSearchIndex> productVariantElasticRepository1, IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> recevingOrderSearchIndexRepository1, IRedisRepository redisRepository)
        {
            _authorizationService = authorizationService;
            _recevingOrderRepository = recevingOrderRepository;
            _userAuthentication = userAuthentication;
            _poRepository = poRepository;
            _packageRepository = packageRepository;
            _notificationService = notificationService;
            _locationRepository = locationRepository;
            _productVariantRepository = productVariantRepository;
            _productVariantElasticRepository = productVariantElasticRepository1;
            _recevingOrderSearchIndexRepository = recevingOrderSearchIndexRepository1;
            _redisRepository = redisRepository;
        }
        
        [HttpPut("api/goodsreceipt/update")]
        [SwaggerOperation(
            Summary = "Update Receiving Order",
            Description = "Update Receiving Order",
            OperationId = "gr.update",
            Tags = new[] { "GoodsReceiptOrders" })
        ]

        public override async Task<ActionResult<ROUpdateResponse>> HandleAsync(ROUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Update))
                return Unauthorized();
            
            var ro = new GoodsReceiptOrder();
            
            //Check if creating for first time for elastic and addasync procedure

            //Create new transaction if null
            if (ro.Transaction == null)
                ro.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.GoodsReceipt, ro.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
            
                                          
            ro.Transaction = TransactionUpdateHelper.UpdateTransaction(ro.Transaction,UserTransactionActionType.Modify,TransactionType.GoodsReceipt,
                (await _userAuthentication.GetCurrentSessionUser()).Id, ro.Id, "");
            
            var purhchaseOrder = await _poRepository.GetByIdAsync(request.PurchaseOrderNumber);
            if (purhchaseOrder == null)
                return NotFound("Can not find PurchaseOrder of id :" + request.PurchaseOrderNumber);
            
            ro.SupplierId = purhchaseOrder.SupplierId;
            ro.PurchaseOrderId = request.PurchaseOrderNumber;

            var location = await _locationRepository.GetByIdAsync(request.LocationId);
            if (location == null)
                return NotFound("ID of Location not found");
            
            ro.Location = location;
                
            //TODO: Update sku of product in a seperate API
            
            //Data of receipt order is from frontend

           GoodsReceiptBusinessService gbs = new GoodsReceiptBusinessService( _productVariantRepository, _recevingOrderRepository, _packageRepository, _productVariantElasticRepository,
               _recevingOrderSearchIndexRepository, _poRepository,_redisRepository);
           ro = await gbs.ReceiveProducts(ro, request.UpdateItems);
            await _recevingOrderSearchIndexRepository.ElasticSaveSingleAsync(true,IndexingHelper.GoodsReceiptOrderSearchIndex(ro), ElasticIndexConstant.RECEIVING_ORDERS);

            var response = new ROUpdateResponse();
            response.CreatedGoodsReceiptId = ro.Id;
            response.TransactionId = ro.TransactionId;

            return Ok(response);
        }
    }
    
     public class ReceivingOrderUpdateProductItem : BaseAsyncEndpoint.WithRequest<ROSingleProductUpdateRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ProductVariantSearchIndex> _poSearchIndexRepository;

        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;
        private readonly IUserSession _userAuthentication;

        private readonly INotificationService _notificationService;
        private readonly IRedisRepository _redisRepository;

        public ReceivingOrderUpdateProductItem(IAuthorizationService authorizationService, IAsyncRepository<ProductVariantSearchIndex> poSearchIndexRepository, IAsyncRepository<ProductVariant> productVariantRepository, IUserSession userAuthentication, INotificationService notificationService, IRedisRepository redisRepository)
        {
            _authorizationService = authorizationService;
            _poSearchIndexRepository = poSearchIndexRepository;
            _productVariantRepository = productVariantRepository;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _redisRepository = redisRepository;
        }


        [HttpPost("api/goodsreceipt/productinfoupdate")]
        [SwaggerOperation(
            Summary = "Update Receiving Order",
            Description = "Update Receiving Order",
            OperationId = "gr.updateitem",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult> HandleAsync(ROSingleProductUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
        
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Update))
                return Unauthorized();


            await _redisRepository.AddProductUpdateMessage(new ProductUpdateMessage
            {
                Sku = request.Sku,
                ProductVariantId = request.ProductVariantId
            });
         
            return Ok();
        }
    }
     
      public class ReceivingOrderSubmit : BaseAsyncEndpoint.WithRequest<ROSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private readonly ILogger<ReceivingOrderSubmit> _logger;
        private readonly IAuthorizationService _authorizationService;


        private IAsyncRepository<PurchaseOrder> _poAsyncRepository;
        private IElasticAsyncRepository<PurchaseOrderSearchIndex> _poSearchIndexAsyncRepository;
        private IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> _roSearchIndexAsyncRepository;
        
        private readonly INotificationService _notificationService;
        private readonly IUserSession _userAuthentication;


        public ReceivingOrderSubmit(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository, INotificationService notificationService, IUserSession userAuthentication, ILogger<ReceivingOrderSubmit> logger, IElasticAsyncRepository<PurchaseOrderSearchIndex> poSearchIndexAsyncRepository1, IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> roSearchIndexAsyncRepository1, IAuthorizationService authorizationService)
        {
            _roAsyncRepository = roAsyncRepository;
            _poAsyncRepository = poAsyncRepository;
            _notificationService = notificationService;
            _userAuthentication = userAuthentication;
            _logger = logger;
            _poSearchIndexAsyncRepository = poSearchIndexAsyncRepository1;
            _roSearchIndexAsyncRepository = roSearchIndexAsyncRepository1;
            _authorizationService = authorizationService;
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
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Approve))
                return Unauthorized();
            
            var ro = await _roAsyncRepository.GetByIdAsync(request.ReceivingOrderId);
            if (ro == null)
                return NotFound("Can not find GoodsReceipt of id :" + request.ReceivingOrderId);
            
            var po = await _poAsyncRepository.GetByIdAsync(ro.PurchaseOrderId);
            ro.Transaction = TransactionUpdateHelper.UpdateTransaction(ro.Transaction,UserTransactionActionType.Submit,TransactionType.GoodsReceipt,
                (await _userAuthentication.GetCurrentSessionUser()).Id, ro.Id, "");

            var response = new ROSubmitResponse();
            BigQueryService bigQueryService = new BigQueryService();

            GoodsReceiptBusinessService gbs = new GoodsReceiptBusinessService(
                _roAsyncRepository, _poAsyncRepository);

            var insufficientVariantsId = (await gbs.CheckSufficientReceiptQuantity(ro)); 
            if (insufficientVariantsId.Count > 0)
            {
                response.IncompletePurchaseOrderId = po.Id;
                response.IncompleteVariantId = insufficientVariantsId;
            }
            
            foreach (var goodsReceiptOrderItem in ro.ReceivedOrderItems)
            {
                try
                {
                    bigQueryService.InsertProductRowBQ(goodsReceiptOrderItem.ProductVariant, ro.PurchaseOrder.PurchaseOrderProduct.FirstOrDefault(p => p.ProductVariantId == goodsReceiptOrderItem.ProductVariantId).Price
                        , ro.Location.LocationName, goodsReceiptOrderItem.QuantityReceived , 0, 0, "In Storage", ro.Supplier.SupplierName);
                    _logger.LogInformation("Updated BigQuery on " + this.GetType().ToString());
                }
                catch
                {
                    _logger.LogError("Error updating BigQuery on " + this.GetType().ToString());
                }
            }
            
            if(response.IncompleteVariantId.IsNullOrEmpty())
                po.PurchaseOrderStatus = PurchaseOrderStatusType.Done;
            
            await _poAsyncRepository.UpdateAsync(po);
            await _roAsyncRepository.UpdateAsync(ro);
            await _poSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po),ElasticIndexConstant.PURCHASE_ORDERS);
            await _roSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.GoodsReceiptOrderSearchIndex(ro),ElasticIndexConstant.RECEIVING_ORDERS);
            
             
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Goods Receipt", ro.Id);
                
            await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.MANAGER,
                currentUser.Id, messageNotification, PageConstant.GOODSRECEIPT, po.Id);
            
            await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.ACCOUNTANT,
                currentUser.Id, messageNotification, PageConstant.GOODSRECEIPT, po.Id);
            
            if(!response.IncompleteVariantId.IsNullOrEmpty())
                return Ok(response);
            return Ok();
        }
    }
      
    public class ReceivingOrderSkuExistanceCheck : BaseAsyncEndpoint.WithRequest<ROSKUExistanceRequest>.WithResponse<ROSKUExistanceResponse>
    {
        private readonly IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private readonly IAsyncRepository<PurchaseOrder> _poAsyncRepository;

        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _poElasticRepository;

        private readonly IRedisRepository _redisRepository;        
        private readonly IAuthorizationService _authorizationService;

        public ReceivingOrderSkuExistanceCheck(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IRedisRepository redisRepository, IAuthorizationService authorizationService, IElasticAsyncRepository<PurchaseOrderSearchIndex> poElasticRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository)
        {
            _roAsyncRepository = roAsyncRepository;
            _redisRepository = redisRepository;
            _authorizationService = authorizationService;
            _poElasticRepository = poElasticRepository;
            _poAsyncRepository = poAsyncRepository;
        }

        [HttpPost("api/goodsreceipt/skuexistance")]
        [SwaggerOperation(
            Summary = "Submit Goods Receipt Order",
            Description = "Submit Goods Receipt Order",
            OperationId = "gr.submit",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult<ROSKUExistanceResponse>> HandleAsync(ROSKUExistanceRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Read))
                return Unauthorized();
            
            // var ro = await _roAsyncRepository.GetByIdAsync(request.ReceiptPurchaseOrderId);
            // if (ro == null)
            //     return NotFound("Can not found goods receipt with ID: " + request.ReceiptPurchaseOrderId);
            
            var po = await _poAsyncRepository.GetByIdAsync(request.ReceiptPurchaseOrderId);
            
            if (po == null)
                return NotFound("Can not find purchase order from goodsReceipt");
            
            
            GoodsReceiptBusinessService gbs = new GoodsReceiptBusinessService(
                _redisRepository, _roAsyncRepository, _poElasticRepository);

            var response = new ROSKUExistanceResponse();
            response.ExistRedisVariantSkus = await gbs.CheckSkuExistance(po);
            
            return Ok(response);
        }
    }
}