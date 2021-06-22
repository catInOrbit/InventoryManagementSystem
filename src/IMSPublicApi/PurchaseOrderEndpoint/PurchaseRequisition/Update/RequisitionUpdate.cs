using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseRequisition.Update
{
    public class RequisitionUpdate : BaseAsyncEndpoint.WithRequest<RUpdateRequest>.WithResponse<RUpdateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _pvasyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;

        
        private readonly INotificationService _notificationService;

        public RequisitionUpdate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, IAsyncRepository<ProductVariant> pvasyncRepository, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _indexAsyncRepository = indexAsyncRepository;
            _pvasyncRepository = pvasyncRepository;
            _notificationService = notificationService;
        }

        [HttpPut("api/requisition/update")]
        [SwaggerOperation(
            Summary = "Update Requsition as role Saleman",
            Description = "Submit Requsition as role Saleman ",
            OperationId = "r.update",
            Tags = new[] { "RequisitionEndpoints" })
        ]

        public override async Task<ActionResult<RUpdateResponse>> HandleAsync(RUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Requisition", UserOperations.Update))
                return Unauthorized();
                
            var po = new ApplicationCore.Entities.Orders.PurchaseOrder();
            po.Transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.Requisition,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                TransactionStatus = true
            };

            po.PurchaseOrderStatus = PurchaseOrderStatusType.RequisitionCreated;
            
            foreach (var requestOrderItem in request.OrderItems)
            {
                requestOrderItem.OrderId = po.Id;
                requestOrderItem.ProductVariant = await _pvasyncRepository.GetByIdAsync(requestOrderItem.ProductVariantId);
                requestOrderItem.TotalAmount = requestOrderItem.OrderQuantity * requestOrderItem.Price;
            }
            
            po.PurchaseOrderProduct = request.OrderItems;
            
            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            po.Deadline = request.Deadline;
            
            await _asyncRepository.AddAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(true, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create","Purchase Requisition", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok(new RUpdateResponse{CreatedRequisitionId = po.Id});
        }
    }
}