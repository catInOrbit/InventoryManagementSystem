using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseRequisition.Create
{
    public class RequisitionCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<RCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;

        private readonly INotificationService _notificationService;

        public RequisitionCreate(IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _indexAsyncRepository = indexAsyncRepository;
            _notificationService = notificationService;
        }

        [HttpPost("api/requisition/create")]
        [SwaggerOperation(
            Summary = "Create Requsition as role Saleman",
            Description = "Create Requsition as role Saleman",
            OperationId = "r.create",
            Tags = new[] { "RequisitionEndpoints" })
        ]
        public override async Task<ActionResult<RCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Requisition", UserOperations.Create))
                return Unauthorized();

            var po = new ApplicationCore.Entities.Orders.PurchaseOrder();
            po.Transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.PriceQuote,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                TransactionStatus = true
            };

            po.PurchaseOrderStatus = PurchaseOrderStatusType.RequisitionCreated;
            
            await _asyncRepository.AddAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(true, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            var response = new RCreateResponse();
            response.PurchaseOrder = po;
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create","Purchase Requisition", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok(response);
        }
    }
}