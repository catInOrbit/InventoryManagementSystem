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
        private readonly IUserSession _userAuthentication;
        
        private readonly INotificationService _notificationService;


        public ReceivingOrderUpdate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> recevingOrderRepository, IAsyncRepository<ProductVariant> productRepository, IUserSession userAuthentication, IAsyncRepository<GoodsReceiptOrderSearchIndex> recevingOrderSearchIndexRepository, IAsyncRepository<PurchaseOrder> poRepository, IAsyncRepository<Package> packageRepository, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _recevingOrderRepository = recevingOrderRepository;
            _productRepository = productRepository;
            _userAuthentication = userAuthentication;
            _recevingOrderSearchIndexRepository = recevingOrderSearchIndexRepository;
            _poRepository = poRepository;
            _packageRepository = packageRepository;
            _notificationService = notificationService;
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
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Update))
                return Unauthorized();
            
            var ro = new GoodsReceiptOrder();
            
            //Check if creating for first time for elastic and addasync procedure

            //Create new transaction if null
            if (ro.Transaction == null)
                ro.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.GoodsReceipt, ro.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
            
                                          
            ro.Transaction = TransactionUpdateHelper.UpdateTransaction(ro.Transaction,UserTransactionActionType.Modify, ro.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);
            
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
            }

            //Update and indexing
            await _recevingOrderRepository.AddAsync(ro);
            
            
            foreach (var roi in ro.ReceivedOrderItems)
            {
                var package = (await _packageRepository.ListAllAsync(new PagingOption<Package>(0, 0))).ResultList.FirstOrDefault(package => package.ProductVariantId == roi.ProductVariantId);
                
                //Package
                package = new Package
                {
                    ProductVariantId =  roi.ProductVariantId,
                    Quantity = roi.QuantityReceived,
                    TotalPrice = roi.ProductVariant.Price * roi.QuantityReceived,
                    Location = ro.StorageLocationReceipt,
                    ImportedDate = ro.ReceivedDate,
                    GoodsReceiptOrderId = ro.Id
                };
                
                await _packageRepository.AddAsync(package);
            }
           
            await _recevingOrderSearchIndexRepository.ElasticSaveSingleAsync(true,IndexingHelper.GoodsReceiptOrderSearchIndex(ro), ElasticIndexConstant.RECEIVING_ORDERS);

            var response = new ROUpdateResponse();
            response.CreatedGoodsReceiptId = ro.Id;
            response.TransactionId = ro.TransactionId;

            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update","Goods Receipt", ro.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
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

        public ReceivingOrderUpdateProductItem(IAuthorizationService authorizationService, IAsyncRepository<ProductVariantSearchIndex> poSearchIndexRepository, IAsyncRepository<ProductVariant> productVariantRepository, IUserSession userAuthentication, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _poSearchIndexRepository = poSearchIndexRepository;
            _productVariantRepository = productVariantRepository;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
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
        
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Update))
                return Unauthorized();
            
            //Get Product Variant
            var productVariant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId);
            
            //Update SKU, LOCATION, PRICE
            productVariant.Sku = request.Sku;
            productVariant.Price = request.SalePrice;
            
            //Update and indexing
            await _productVariantRepository.UpdateAsync(productVariant);
            await _poSearchIndexRepository.ElasticSaveSingleAsync(false,IndexingHelper.ProductVariantSearchIndex(productVariant), ElasticIndexConstant.RECEIVING_ORDERS);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update","Product Variant", productVariant.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok();
        }
    }
     
      public class ReceivingOrderSubmit : BaseAsyncEndpoint.WithRequest<ROSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private IAsyncRepository<PurchaseOrder> _poAsyncRepository;
        private IAsyncRepository<PurchaseOrderSearchIndex> _poSearchIndexAsyncRepository;
        private IAsyncRepository<GoodsReceiptOrderSearchIndex> _roSearchIndexAsyncRepository;
        
        private readonly INotificationService _notificationService;
        private readonly IUserSession _userAuthentication;


        public ReceivingOrderSubmit(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository, IAsyncRepository<PurchaseOrderSearchIndex> poSearchIndexAsyncRepository, IAsyncRepository<GoodsReceiptOrderSearchIndex> roSearchIndexAsyncRepository, INotificationService notificationService, IUserSession userAuthentication)
        {
            _roAsyncRepository = roAsyncRepository;
            _poAsyncRepository = poAsyncRepository;
            _poSearchIndexAsyncRepository = poSearchIndexAsyncRepository;
            _roSearchIndexAsyncRepository = roSearchIndexAsyncRepository;
            _notificationService = notificationService;
            _userAuthentication = userAuthentication;
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
            ro.Transaction = TransactionUpdateHelper.UpdateTransaction(ro.Transaction,UserTransactionActionType.Submit, ro.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);

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
            await _poSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po),ElasticIndexConstant.PURCHASE_ORDERS);
            await _roSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.GoodsReceiptOrderSearchIndex(ro),ElasticIndexConstant.RECEIVING_ORDERS);
            
             
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Goods Receipt", ro.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            
            if(!response.IncompleteVariantId.IsNullOrEmpty())
                return Ok(response);
            return Ok();
        }
    }
    
    
}