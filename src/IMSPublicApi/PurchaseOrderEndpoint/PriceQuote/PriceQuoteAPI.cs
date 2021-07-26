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
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<Product> _productRepos;
        
        private INotificationService _notificationService;


        public PriceQuoteRequestCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication,  IAsyncRepository<Product> productRepos, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productRepos = productRepos;
            _indexAsyncRepository = indexAsyncRepository;
            _notificationService = notificationService;
        }
        
        [HttpPost("api/pricequote/create")]
        [SwaggerOperation(
            Summary = "Create price quote request with Id from purchase requisition",
            Description = "Create price quote request with Id from purchase requisition",
            OperationId = "po.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]

        public override async Task<ActionResult<PQCreateResponse>> HandleAsync(PQCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Create))
                return Unauthorized();
            
            var response = new PQCreateResponse();
            var po = await _asyncRepository.GetByIdAsync(request.Id);
            
            // po.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.PriceQuote, po.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Modify,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");

            po.PurchaseOrderStatus = PurchaseOrderStatusType.PriceQuote;
                            
            response.PurchaseOrder = po;
            Console.WriteLine(po.ToString());
            
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false,IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
            foreach (var orderItem in po.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;
                  
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create", "Price Quote", po.Id);
            await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.MANAGER,
                currentUser.Id, messageNotification);
            await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.ACCOUNTANT,
                currentUser.Id, messageNotification);

            response.MergedOrderIdLists = _asyncRepository.GetMergedPurchaseOrders(po.MergedWithPurchaseOrderId);
            return Ok(response);
        }
    }
    
        public class PriceQuoteRequestEdit : BaseAsyncEndpoint.WithRequest<PQEditRequest>.WithResponse<PQEditResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepos;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        
        private INotificationService _notificationService;

        public PriceQuoteRequestEdit(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IAsyncRepository<ProductVariant> productVariantRepos, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productVariantRepos = productVariantRepos;
            _indexAsyncRepository = indexAsyncRepository;
            _notificationService = notificationService;
        }
        
        
        [HttpPut("api/pricequote/edit")]
        [SwaggerOperation(
            Summary = "Edit price quote request",
            Description = "Edit price quote request",
            OperationId = "po.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]
        public override async Task<ActionResult<PQEditResponse>> HandleAsync(PQEditRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Update))
                return Unauthorized();

            var po = _asyncRepository.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction, UserTransactionActionType.Modify,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");

            po.PurchaseOrderProduct.Clear();
            po.TotalOrderAmount = 0;
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
            {
                requestOrderItemInfo.OrderId = po.Id;
                requestOrderItemInfo.ProductVariant = await _productVariantRepos.GetByIdAsync(requestOrderItemInfo.ProductVariantId);
                requestOrderItemInfo.TotalAmount = requestOrderItemInfo.Price * requestOrderItemInfo.OrderQuantity;
                po.TotalOrderAmount += requestOrderItemInfo.TotalAmount;
                po.PurchaseOrderProduct.Add(requestOrderItemInfo);
            }
            
            Console.WriteLine(po.ToString());
            ApplicationUser currentUser;
            string messageNotification;
            bool mergePerformed = false;
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
                await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                    currentUser.Id, messageNotification);
            }
            
            po.MailDescription = request.MailDescription;
            po.SupplierId = request.SupplierId;
            po.Deadline = request.Deadline;
            
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
            response.MergedOrderIdLists = _asyncRepository.GetMergedPurchaseOrders(po.MergedWithPurchaseOrderId);
            return Ok(response);
        }
    }
        
          public class PriceQuoteRequestSubmit : BaseAsyncEndpoint.WithRequest<PQSubmitRequest>.WithResponse<PQSubmitResponse>
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        
        private readonly IRedisRepository _redisRepository;
        private readonly IAsyncRepository<Notification> _notificationAsyncRepository;
        private IHubContext<NotificationHub> _hubContext;

        private readonly INotificationService _notificationService;

        public PriceQuoteRequestSubmit(IEmailSender emailSender, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, IRedisRepository redisRepository, IAsyncRepository<Notification> notificationAsyncRepository, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
        {
            _emailSender = emailSender;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _indexAsyncRepository = indexAsyncRepository;
            _redisRepository = redisRepository;
            _notificationAsyncRepository = notificationAsyncRepository;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }
        
        [HttpPost("api/pricequote/submit")]
        [SwaggerOperation(
            Summary = "Submit price quote request",
            Description = "Submit price quote request",
            OperationId = "po.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]
        public override async Task<ActionResult<PQSubmitResponse>> HandleAsync(PQSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
          
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Update))
                return Unauthorized();
            var response = new PQSubmitResponse();
            var po = _asyncRepository.GetPurchaseOrderByNumber(request.OrderNumber);
            po.PurchaseOrderStatus = PurchaseOrderStatusType.PurchaseOrder;
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Submit,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
            
            
            await _asyncRepository.UpdateAsync(po);
            
            // var subject = "REQUEST FOR QUOTATION-" + DateTime.UtcNow.ToString("dd/MM//yyyy") + " FROM IMS Inventory";
            
            // var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            // var message = new EmailMessage(request.To, subject, request.Content, files);
            // await _emailSender.SendEmailAsync(message);
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

            foreach (var orderItem in po.PurchaseOrderProduct)
                orderItem.IsShowingProductVariant = true;
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Price Quote", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            response.PurchaseOrder = po;
            return Ok(response);
        }
    }

}