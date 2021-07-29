using System;
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

        private readonly IAsyncRepository<ProductVariant> _productRepository;
        private readonly IUserSession _userAuthentication;
        
        private readonly INotificationService _notificationService;


        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> recevingOrderRepository, IAsyncRepository<ProductVariant> productRepository, IUserSession userAuthentication,  IAsyncRepository<PurchaseOrder> poRepository, IAsyncRepository<Package> packageRepository, INotificationService notificationService, IAsyncRepository<Location> locationRepository, IAsyncRepository<ProductVariant> productVariantRepository, IElasticAsyncRepository<ProductVariantSearchIndex> productVariantElasticRepository1, IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> recevingOrderSearchIndexRepository1)
        {
            _authorizationService = authorizationService;
            _recevingOrderRepository = recevingOrderRepository;
            _productRepository = productRepository;
            _userAuthentication = userAuthentication;
            _poRepository = poRepository;
            _packageRepository = packageRepository;
            _notificationService = notificationService;
            _locationRepository = locationRepository;
            _productVariantRepository = productVariantRepository;
            _productVariantElasticRepository = productVariantElasticRepository1;
            _recevingOrderSearchIndexRepository = recevingOrderSearchIndexRepository1;
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
            
                                          
            ro.Transaction = TransactionUpdateHelper.UpdateTransaction(ro.Transaction,UserTransactionActionType.Modify,
                (await _userAuthentication.GetCurrentSessionUser()).Id, ro.Id, "");
            
            var purhchaseOrder = await _poRepository.GetByIdAsync(request.PurchaseOrderNumber);
            ro.SupplierId = purhchaseOrder.SupplierId;
            ro.PurchaseOrderId = request.PurchaseOrderNumber;

            var location = await _locationRepository.GetByIdAsync(request.LocationId);
            if (location == null)
                return NotFound("ID of Location not found");
            
            ro.Location = location;
                
            //TODO: Update sku of product in a seperate API
            
            //Data of receipt order is from frontend

            ro.ReceivedOrderItems.Clear();
            foreach (var item in request.UpdateItems)
            {
                if (item.QuantityReceived > 0)
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
                
                    var productVariant = await _productVariantRepository.GetByIdAsync(roi.ProductVariantId);
                    bool initiateUpdate = false;
                    if (item.Sku != null)
                    {
                        productVariant.Sku = item.Sku;
                        initiateUpdate = true;
                    }

                    if (item.Barcode != null)
                    {
                        productVariant.Barcode = item.Barcode;
                        initiateUpdate = true;
                    }

                    if (initiateUpdate)
                    {
                        await _productVariantRepository.UpdateAsync(productVariant);
                        await _productVariantElasticRepository.ElasticSaveSingleAsync(false,
                            IndexingHelper.ProductVariantSearchIndex(productVariant),
                            ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
                    }
                }
            }

            //Update and indexing
            await _recevingOrderRepository.AddAsync(ro);
            
            foreach (var roi in ro.ReceivedOrderItems)
            {
                //Package
                var package = new Package
                {
                    ProductVariantId =  roi.ProductVariantId,
                    Quantity = roi.QuantityReceived,
                    Location = ro.Location,
                    ImportedDate = ro.ReceivedDate,
                    GoodsReceiptOrderId = ro.Id,
                    Price = ro.PurchaseOrder.PurchaseOrderProduct.FirstOrDefault(item => item.ProductVariantId == roi.ProductVariantId).Price,
                    SupplierId = ro.SupplierId,
                };
                package.TotalPrice = package.Price * package.Quantity;
                package.LatestUpdateDate = DateTime.UtcNow;

                await _packageRepository.AddAsync(package);
                roi.ProductVariant.Packages.Add(package);

               
                
                //Begin Inserting into bigQuery

            }
            await _recevingOrderSearchIndexRepository.ElasticSaveSingleAsync(true,IndexingHelper.GoodsReceiptOrderSearchIndex(ro), ElasticIndexConstant.RECEIVING_ORDERS);


            var response = new ROUpdateResponse();
            response.CreatedGoodsReceiptId = ro.Id;
            response.TransactionId = ro.TransactionId;

            // var currentUser = await _userAuthentication.GetCurrentSessionUser();
            //
            // var messageNotification =
            //     _notificationService.CreateMessage(currentUser.Fullname, "Update","Goods Receipt", ro.Id);
            //     
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification);
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


            await _redisRepository.AddProductUpdateMessage("ProductUpdateMessage", new ProductUpdateMessage
            {
                Barcode = request.Barcode,
                Sku = request.Sku,
                ProductVariantId = request.ProductVariantId
            });
            
            //Get Product Variant
            // var productVariant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId);
            //
            // //Update SKU, LOCATION, PRICE
            // productVariant.Sku = request.Sku;
            // productVariant.Barcode = request.Barcode;
            
            //Update and indexing
            // await _productVariantRepository.UpdateAsync(productVariant);
            // await _poSearchIndexRepository.ElasticSaveSingleAsync(false,IndexingHelper.ProductVariantSearchIndex(productVariant), ElasticIndexConstant.RECEIVING_ORDERS);
            
            // var currentUser = await _userAuthentication.GetCurrentSessionUser();
            //
            // var messageNotification =
            //     _notificationService.CreateMessage(currentUser.Fullname, "Update","Product Variant", request.ProductVariantId);
            //     
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification);
            return Ok();
        }
    }
     
      public class ReceivingOrderSubmit : BaseAsyncEndpoint.WithRequest<ROSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private readonly ILogger<ReceivingOrderSubmit> _logger;


        private IAsyncRepository<PurchaseOrder> _poAsyncRepository;
        private IElasticAsyncRepository<PurchaseOrderSearchIndex> _poSearchIndexAsyncRepository;
        private IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> _roSearchIndexAsyncRepository;
        
        private readonly INotificationService _notificationService;
        private readonly IUserSession _userAuthentication;


        public ReceivingOrderSubmit(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository, INotificationService notificationService, IUserSession userAuthentication, ILogger<ReceivingOrderSubmit> logger, IElasticAsyncRepository<PurchaseOrderSearchIndex> poSearchIndexAsyncRepository1, IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> roSearchIndexAsyncRepository1)
        {
            _roAsyncRepository = roAsyncRepository;
            _poAsyncRepository = poAsyncRepository;
            _notificationService = notificationService;
            _userAuthentication = userAuthentication;
            _logger = logger;
            _poSearchIndexAsyncRepository = poSearchIndexAsyncRepository1;
            _roSearchIndexAsyncRepository = roSearchIndexAsyncRepository1;
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
            ro.Transaction = TransactionUpdateHelper.UpdateTransaction(ro.Transaction,UserTransactionActionType.Submit,
                (await _userAuthentication.GetCurrentSessionUser()).Id, ro.Id, "");

            var response = new ROSubmitResponse();
            BigQueryService bigQueryService = new BigQueryService();
            foreach (var goodsReceiptOrderItem in ro.ReceivedOrderItems)
            {
                if (goodsReceiptOrderItem.QuantityReceived < po.PurchaseOrderProduct
                    .FirstOrDefault(orderItem => orderItem.ProductVariantId == goodsReceiptOrderItem.ProductVariantId)
                    .OrderQuantity)
                {
                    response.IncompletePurchaseOrderId = po.Id;
                    response.IncompleteVariantId.Add(goodsReceiptOrderItem.ProductVariantId);
                }

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
}