using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
        public class PurchaseOrderDelete : BaseAsyncEndpoint.WithRequest<PORejectRequest>.WithResponse<PORejectResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _poSearchRepos;
        
        private readonly INotificationService _notificationService;
        public PurchaseOrderDelete(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, INotificationService notificationService, IElasticAsyncRepository<PurchaseOrderSearchIndex> poSearchRepos1)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _poSearchRepos = poSearchRepos1;
        }
        
        [HttpPut("api/purchaseorder/reject")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "pq.cancel",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<PORejectResponse>> HandleAsync(PORejectRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Reject))
                return Unauthorized();

            var po = await _asyncRepository.GetByIdAsync(request.Id);
            if (po == null)
                return NotFound("Can not find PurchaseOrder of id :" + request.Id);
            
            po.Transaction.TransactionStatus = false;

            if((int)po.PurchaseOrderStatus >= 0 && (int)po.PurchaseOrderStatus <= 2)
                po.PurchaseOrderStatus = PurchaseOrderStatusType.RequisitionCanceled;
            
            else if((int)po.PurchaseOrderStatus == 3)
                po.PurchaseOrderStatus = PurchaseOrderStatusType.PQCanceled;
            
            else if((int)po.PurchaseOrderStatus >=4 && (int)po.PurchaseOrderStatus <= 5)
                po.PurchaseOrderStatus = PurchaseOrderStatusType.POCanceled;

            else
            {
                return NotFound("Unable to cancel at this stage");
            }
            
            // if((int)po.PurchaseOrderStatus == 0)
            //     po.PurchaseOrderStatus = PurchaseOrderStatusType.RequisitionCanceled;
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Reject,TransactionType.Purchase,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, request.CancelReason);
            
           await _asyncRepository.UpdateAsync(po,cancellationToken);
           await _poSearchRepos.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
           
           var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
           var messageNotification =
               _notificationService.CreateMessage(currentUser.Fullname, "Cancel","Purchase Order", po.Id);
           
           await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.ACCOUNTANT,
               currentUser.Id, messageNotification, PageConstant.PURCHASEORDER, po.Id);
           
           await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.SALEMAN,
               currentUser.Id, messageNotification, PageConstant.PURCHASEORDER, po.Id);

           foreach (var orderItem in po.PurchaseOrderProduct)
               orderItem.IsShowingProductVariant = true;
           
           return Ok(new PORejectResponse()
           {
               PurchaseOrder = po
           });
        }
    }
        
    public class PurchaseOrderConfirm : BaseAsyncEndpoint.WithRequest<POConfirmRequest>.WithResponse<POConfirmResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _poSearchRepos;
        private readonly IUserSession _userAuthentication;


        private readonly IAuthorizationService _authorizationService;
        
        private INotificationService _notificationService;

        public PurchaseOrderConfirm(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService, INotificationService notificationService, IUserSession userAuthentication, IElasticAsyncRepository<PurchaseOrderSearchIndex> poSearchRepos1)
        {
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _userAuthentication = userAuthentication;
            _poSearchRepos = poSearchRepos1;
        }

        [HttpPost("api/po/confirm")]
        [SwaggerOperation(
            Summary = "Confirm purchase order ",
            Description = "Confirm purchase order",
            OperationId = "po.confirm",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<POConfirmResponse>> HandleAsync(POConfirmRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Approve))
                return Unauthorized();
            
            var po = await _purchaseOrderRepos.GetByIdAsync(request.PurchaseOrderNumber);
            if (po == null)
                return NotFound("Can not find PurchaseOrder of id :" + request.PurchaseOrderNumber);
            
            po.PurchaseOrderStatus = PurchaseOrderStatusType.POConfirm;
            await _purchaseOrderRepos.UpdateAsync(po);
            await _poSearchRepos.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Confirm,TransactionType.Purchase,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
            
            // var currentUser = await _userAuthentication.GetCurrentSessionUser();
            //     
            // var messageNotification =
            //     _notificationService.CreateMessage(currentUser.Fullname, "Confirm","Purchase Order", po.Id);
            //     
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification);

            foreach (var orderItem in po.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;
            
            var response = new POConfirmResponse()
            {
                PurchaseOrder = po
            };
            return Ok(response);
        }
    }
    
        public class PurchaseOrderCreate : BaseAsyncEndpoint.WithRequest<POCreateRequest>.WithResponse<POCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        private readonly IUserSession _userAuthentication;

        
        private readonly INotificationService _notificationService;

        public PurchaseOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IUserSession userAuthentication,  INotificationService notificationService, IElasticAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository1)
        {
            _authorizationService = authorizationService;
            _purchaseOrderRepos = purchaseOrderRepos;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _indexAsyncRepository = indexAsyncRepository1;
        }

        [HttpPost("api/purchaseorder/create")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "po.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<POCreateResponse>> HandleAsync(POCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new POCreateResponse();

            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Create))
                return Unauthorized();
            

            var poData = await _purchaseOrderRepos.GetByIdAsync(request.PurchaseOrderNumber);
            if (poData == null)
                return NotFound("Can not find PurchaseOrder of id :" + request.PurchaseOrderNumber);
            
            poData.PurchaseOrderStatus = PurchaseOrderStatusType.PurchaseOrder;
            poData.Transaction.CurrentType = TransactionType.Purchase;

            poData.Transaction = TransactionUpdateHelper.UpdateTransaction(poData.Transaction,UserTransactionActionType.Create,TransactionType.Purchase,
                (await _userAuthentication.GetCurrentSessionUser()).Id, poData.Id, "");
            response.PurchaseOrder = poData;
            await _purchaseOrderRepos.UpdateAsync(poData);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(poData), ElasticIndexConstant.PURCHASE_ORDERS);
            
            // var currentUser = await _userAuthentication.GetCurrentSessionUser();
            //     
            // var messageNotification =
            //     _notificationService.CreateMessage(currentUser.Fullname, "Create","Purchase Order", poData.Id);
            //     
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification);
            return Ok(response);
        }
    }
        
    public class PurchaseOrderSubmit : BaseAsyncEndpoint.WithRequest<POSubmitRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _poIndexAsyncRepositoryRepos;

        private readonly INotificationService _notificationService;
        private readonly ILogger<PurchaseOrderSubmit> _logger;


        public PurchaseOrderSubmit(IEmailSender emailSender, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication,  INotificationService notificationService, ILogger<PurchaseOrderSubmit> logger, IElasticAsyncRepository<PurchaseOrderSearchIndex> poIndexAsyncRepositoryRepos1)
        {
            _emailSender = emailSender;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _logger = logger;
            _poIndexAsyncRepositoryRepos = poIndexAsyncRepositoryRepos1;
        }
        
        [HttpPost("api/purchaseorder/submit")]
        [SwaggerOperation(
            Summary = "Submit",
            Description = "Submit new purchase order",
            OperationId = "po.submit",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(POSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var subject = "Purchase Order " + DateTime.UtcNow;

            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Update))
                return Unauthorized();

            try
            {
                var po = await _asyncRepository.GetByIdAsync(request.PurchaseOrderNumber);
                if (po == null)
                    return NotFound("Can not find PurchaseOrder of id :" + request.PurchaseOrderNumber);
                
                po.PurchaseOrderStatus = PurchaseOrderStatusType.POWaitingConfirmation;
                po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Submit,TransactionType.Purchase,
                    (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
                
                foreach (var orderItem in po.PurchaseOrderProduct)
                    orderItem.IsShowingProductVariant = true;
                await _asyncRepository.UpdateAsync(po);
                await _poIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
                
                BigQueryService bigQueryService = new BigQueryService();
                
                try
                {
                    foreach (var orderItem in po.PurchaseOrderProduct)
                    {
                        bigQueryService.InsertProductRowBQ(orderItem.ProductVariant, orderItem.Price,null,
                            0, 0 , 0, "Upcoming Order", po.Supplier.SupplierName);
                        _logger.LogInformation("Updated BigQuery on " + this.GetType().ToString());
                    }
                }
                catch
                {
                    _logger.LogError("Error updating BigQuery on " + this.GetType().ToString());
                }
                
                var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, "Submit","Purchase Order", po.Id);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.MANAGER,
                    currentUser.Id, messageNotification, PageConstant.PURCHASEORDER, po.Id);


                var response = new POSubmitResponse();
                response.PurchaseOrder = po;
                response.PurchaseOrder.PurchaseOrderStatusString = response.PurchaseOrder.PurchaseOrderStatus.ToString();
                
                foreach (var orderItem in response.PurchaseOrder.PurchaseOrderProduct)
                    orderItem.IsShowingProductVariant = true;
                return Ok(response);
            }
            catch
            {
                return NotFound();
            }
        }
    }
            
    public class PurchaseOrderUpdate : BaseAsyncEndpoint.WithRequest<POUpdateRequest>.WithResponse<POUpdateResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepos;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _poIndexAsyncRepositoryRepos;

        private readonly IUserSession _userAuthentication;
        private readonly IAuthorizationService _authorizationService;

        private readonly INotificationService _notificationService;

        public PurchaseOrderUpdate(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService,  IUserSession userAuthentication, IAsyncRepository<ProductVariant> productVariantRepos, INotificationService notificationService, IElasticAsyncRepository<PurchaseOrderSearchIndex> poIndexAsyncRepositoryRepos1)
        {
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productVariantRepos = productVariantRepos;
            _notificationService = notificationService;
            _poIndexAsyncRepositoryRepos = poIndexAsyncRepositoryRepos1;
        }
        
        [HttpPut("api/purchaseorder/update")]
        [SwaggerOperation(
            Summary = "Update purchase order",
            Description = "Update purchase order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<POUpdateResponse>> HandleAsync(POUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new POUpdateResponse();
            
            // requires using ContactManager.Authorization;
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Update))
                return Unauthorized();

            var po =  await _purchaseOrderRepos.GetByIdAsync(request.PurchaseOrderNumber);
            if (po == null)
                return NotFound("Can not find PurchaseOrder of id :" + request.PurchaseOrderNumber);          

            
            po.PurchaseOrderProduct.Clear();
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
            {
                requestOrderItemInfo.OrderId = po.Id;
                requestOrderItemInfo.ProductVariant = await _productVariantRepos.GetByIdAsync(requestOrderItemInfo.ProductVariantId);
                requestOrderItemInfo.QuantityLeftAfterReceived = requestOrderItemInfo.OrderQuantity;
                po.TotalOrderAmount += requestOrderItemInfo.Price;
                po.TotalDiscountAmount += requestOrderItemInfo.DiscountAmount;
                po.PurchaseOrderProduct.Add(requestOrderItemInfo);
                po.MailDescription = request.MailDescription;
            }
            po .Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction, UserTransactionActionType.Modify,TransactionType.Purchase,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
            await _purchaseOrderRepos.UpdateAsync(po);
            await _poIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false,IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            response.PurchaseOrder = po;
            
            foreach (var orderItem in po.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;
            
            // var currentUser = await _userAuthentication.GetCurrentSessionUser();
            //     
            // var messageNotification =
            //     _notificationService.CreateMessage(currentUser.Fullname, "Update","Purchase Order", po.Id);
            //     
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification);
            
            return Ok(response);
        }
    }


}