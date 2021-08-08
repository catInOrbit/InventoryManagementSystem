using System;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteRequestCreate : BaseAsyncEndpoint.WithRequest<PQCreateRequest>.WithResponse<PQCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<Product> _productRepos;
        
        private INotificationService _notificationService;


        public PriceQuoteRequestCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication,  IAsyncRepository<Product> productRepos, INotificationService notificationService, IElasticAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productRepos = productRepos;
            _notificationService = notificationService;
            _indexAsyncRepository = indexAsyncRepository;
        }
        
        [HttpPost("api/pricequote/create")]
        [SwaggerOperation(
            Summary = "Create price quote request with Id from purchase requisition",
            Description = "Create price quote request with Id from purchase requisition",
            OperationId = "pq.create",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]

        public override async Task<ActionResult<PQCreateResponse>> HandleAsync(PQCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Create))
                return Unauthorized();
            
            var response = new PQCreateResponse();
            var po = await _asyncRepository.GetByIdAsync(request.Id);
            if (po == null)
                return NotFound("Can not find PriceQuote of id :" + request.Id);
                        
            // po.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.PriceQuote, po.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
            
            po.PurchaseOrderStatus = PurchaseOrderStatusType.PriceQuote;

            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Create,TransactionType.PriceQuote,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");

                            
            response.PurchaseOrder = po;
            
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false,IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
            foreach (var orderItem in po.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;
                  
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create", "Price Quote", po.Id);
            await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.MANAGER,
                currentUser.Id, messageNotification, PageConstant.PRICEQUOTEORDER, po.Id);
            await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.ACCOUNTANT,
                currentUser.Id, messageNotification, PageConstant.PRICEQUOTEORDER, po.Id);

            response.MergedOrderIdLists = _asyncRepository.GetMergedPurchaseOrders(po.Id);
            return Ok(response);
        }
    }
    
        public class PriceQuoteRequestEdit : BaseAsyncEndpoint.WithRequest<PQEditRequest>.WithResponse<PQEditResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepos;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        
        private INotificationService _notificationService;

        public PriceQuoteRequestEdit(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IAsyncRepository<ProductVariant> productVariantRepos, INotificationService notificationService, IElasticAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository1)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productVariantRepos = productVariantRepos;
            _notificationService = notificationService;
            _indexAsyncRepository = indexAsyncRepository1;
        }
        
        
        [HttpPut("api/pricequote/edit")]
        [SwaggerOperation(
            Summary = "Edit price quote request",
            Description = "Edit price quote request",
            OperationId = "pq.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]
        public override async Task<ActionResult<PQEditResponse>> HandleAsync(PQEditRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Update))
                return Unauthorized();

            var po = await _asyncRepository.GetByIdAsync(request.PurchaseOrderNumber);
            if (po == null)
                return NotFound("Can not find PriceQuote of id :" + request.PurchaseOrderNumber);

            po.PurchaseOrderProduct.Clear();
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
            {
                requestOrderItemInfo.OrderId = po.Id;
                requestOrderItemInfo.ProductVariant = await _productVariantRepos.GetByIdAsync(requestOrderItemInfo.ProductVariantId);
                requestOrderItemInfo.TotalAmount = requestOrderItemInfo.Price * requestOrderItemInfo.OrderQuantity;
                requestOrderItemInfo.QuantityLeftAfterReceived = requestOrderItemInfo.OrderQuantity;
                po.TotalOrderAmount += requestOrderItemInfo.TotalAmount;
                po.PurchaseOrderProduct.Add(requestOrderItemInfo);
                po.TotalProductAmount += 1;
            }
            
            ApplicationUser currentUser;
            string messageNotification;
            bool mergePerformed = false;
            if(request.MergedRequisitionIds.Contains(po.Id))
                return BadRequest(
                    "Request list for merging -other- purchase order ID should not contain ID of current Purchase Order: " + po.Id);
            
            foreach (var requestMergedRequisitionId in request.MergedRequisitionIds)
            {
                mergePerformed = true;
                var poMerged = await _asyncRepository.GetByIdAsync(requestMergedRequisitionId);
                poMerged.PurchaseOrderStatus = PurchaseOrderStatusType.RequisitionMerged;
                poMerged.MergedWithPurchaseOrderId = po.Id;
                await _asyncRepository.UpdateAsync(poMerged);
                await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(poMerged), ElasticIndexConstant.PURCHASE_ORDERS);
            }
            
            if(mergePerformed)
            {
                currentUser = await _userAuthentication.GetCurrentSessionUser();
                messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, "Merge", "Price Quote with Ids: " + string.Join(",", request.MergedRequisitionIds), po.Id);
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.SALEMAN,
                    currentUser.Id, messageNotification, PageConstant.PRICEQUOTEORDER, po.Id);
            }
            
            po.MailDescription = request.MailDescription;
            po.SupplierId = request.SupplierId;
            po.Deadline = request.Deadline;
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction, UserTransactionActionType.Modify,TransactionType.PriceQuote,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
            
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

            var response = new PQEditResponse();
            
            foreach (var orderItem in po.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;
            response.PurchaseOrder = po;

            // currentUser = await _userAuthentication.GetCurrentSessionUser();
            // messageNotification = _notificationService.CreateMessage(currentUser.Fullname, "Update", "Price Quote", po.Id);
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification);
            response.MergedOrderIdLists = _asyncRepository.GetMergedPurchaseOrders(po.Id);
            return Ok(response);
        }
    }
        
          public class PriceQuoteRequestSubmit : BaseAsyncEndpoint.WithRequest<PQSubmitRequest>.WithResponse<PQSubmitResponse>
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        
        private readonly IRedisRepository _redisRepository;
        private readonly IAsyncRepository<Notification> _notificationAsyncRepository;
        private IHubContext<NotificationHub> _hubContext;

        private readonly INotificationService _notificationService;

        public PriceQuoteRequestSubmit(IEmailSender emailSender, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IRedisRepository redisRepository, IAsyncRepository<Notification> notificationAsyncRepository, IHubContext<NotificationHub> hubContext, INotificationService notificationService, IElasticAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository1)
        {
            _emailSender = emailSender;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _redisRepository = redisRepository;
            _notificationAsyncRepository = notificationAsyncRepository;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _indexAsyncRepository = indexAsyncRepository1;
        }
        
        [HttpPost("api/pricequote/submit")]
        [SwaggerOperation(
            Summary = "Submit price quote request",
            Description = "Submit price quote request",
            OperationId = "pq.submit",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]
        public override async Task<ActionResult<PQSubmitResponse>> HandleAsync(PQSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
          
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Update))
                return Unauthorized();
            var response = new PQSubmitResponse();
            var po = await _asyncRepository.GetByIdAsync(request.OrderNumber);
            if (po == null)
                return NotFound("Can not find PriceQuote of id :" + request.OrderNumber);
            
            po.PurchaseOrderStatus = PurchaseOrderStatusType.PurchaseOrder;
            
            
            // var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            // var message = new EmailMessage(request.To, subject, request.Content, files);
            // await _emailSender.SendEmailAsync(message);
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction, UserTransactionActionType.Create,TransactionType.Purchase,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

            foreach (var orderItem in po.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Price Quote", po.Id);
                
            await _notificationService.SendNotificationGroup( AuthorizedRoleConstants.MANAGER,
                currentUser.Id, messageNotification, PageConstant.PRICEQUOTEORDER, po.Id);

            response.PurchaseOrder = po;
            return Ok(response);
        }
    }

}