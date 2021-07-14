using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteRequestSubmit : BaseAsyncEndpoint.WithRequest<PQSubmitRequest>.WithoutResponse
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
        public override async Task<ActionResult> HandleAsync([FromForm] PQSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
          
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Update))
                return Unauthorized();
            
            var po = _asyncRepository.GetPurchaseOrderByNumber(request.OrderNumber);
            po.PurchaseOrderStatus = PurchaseOrderStatusType.POCreated;
            po.HasBeenModified = true;
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Submit,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
            
            
            await _asyncRepository.UpdateAsync(po);
            
            var subject = "REQUEST FOR QUOTATION-" + DateTime.UtcNow.ToString("dd/MM//yyyy") + " FROM IMS Inventory";
            
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            var message = new EmailMessage(request.To, subject, request.Content, files);
            await _emailSender.SendEmailAsync(message);
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Price Quote", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok();
        }
    }
}