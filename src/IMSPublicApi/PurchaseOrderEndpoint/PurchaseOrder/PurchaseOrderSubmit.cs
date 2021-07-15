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
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderSubmit : BaseAsyncEndpoint.WithRequest<POSubmitRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _poIndexAsyncRepositoryRepos;

        private readonly INotificationService _notificationService;
        private readonly ILogger<PurchaseOrderSubmit> _logger;


        public PurchaseOrderSubmit(IEmailSender emailSender, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> poIndexAsyncRepositoryRepos, INotificationService notificationService, ILogger<PurchaseOrderSubmit> logger)
        {
            _emailSender = emailSender;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _poIndexAsyncRepositoryRepos = poIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
            _logger = logger;
        }
        
        [HttpPost("api/purchaseorder/submit")]
        [SwaggerOperation(
            Summary = "Submit",
            Description = "Submit new purchase order",
            OperationId = "catalog-items.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromForm] POSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var subject = "Purchase Order " + DateTime.UtcNow;

            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Update))
                return Unauthorized();

            try
            {
                var po = _asyncRepository.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
                // po.PurchaseOrderStatus = PurchaseOrderStatusType.POConfirm;
                po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Submit,
                    (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
                
                await _asyncRepository.UpdateAsync(po);
                var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
                var message = new EmailMessage(request.To, request.Subject, request.Content, files);
                await _emailSender.SendEmailAsync(message);

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
                catch (Exception e)
                {
                    _logger.LogError("Error updating BigQuery on " + this.GetType().ToString());
                }
                
                var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, "Submit","Purchase Order", po.Id);
                
                await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                    currentUser.Id, messageNotification);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
    }
}