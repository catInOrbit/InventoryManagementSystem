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
namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseRequisition
{
    public class RequisitionCreate : BaseAsyncEndpoint.WithRequest<RequisitionCreateRequest>.WithResponse<RequisitionCreateResponse>
     {
         private readonly IAuthorizationService _authorizationService;
         private readonly IUserSession _userAuthentication;
         private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
         private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
         private readonly IAsyncRepository<ProductVariant> _pvasyncRepository;

         private readonly INotificationService _notificationService;

         public RequisitionCreate(IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, INotificationService notificationService, IAsyncRepository<ProductVariant> pvasyncRepository)
         {
             _authorizationService = authorizationService;
             _userAuthentication = userAuthentication;
             _asyncRepository = asyncRepository;
             _indexAsyncRepository = indexAsyncRepository;
             _notificationService = notificationService;
             _pvasyncRepository = pvasyncRepository;
         }

         [HttpPost("api/requisition/create")]
         [SwaggerOperation(
             Summary = "Create Requsition as role Saleman",
             Description = "Create Requsition as role Saleman",
             OperationId = "r.create",
             Tags = new[] { "RequisitionEndpoints" })
         ]
         public override async Task<ActionResult<RequisitionCreateResponse>> HandleAsync(RequisitionCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
         {
              if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Requisition", UserOperations.Create))
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
                  requestOrderItem.Unit = requestOrderItem.Unit;
              }
            
              po.PurchaseOrderProduct = request.OrderItems;
              po.Deadline = request.Deadline;
            
              await _asyncRepository.AddAsync(po);
              await _indexAsyncRepository.ElasticSaveSingleAsync(true, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
              var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
              var messageNotification =
                  _notificationService.CreateMessage(currentUser.Fullname, "Create","Purchase Requisition", po.Id);
                
              await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                  currentUser.Id, messageNotification);
              return Ok(new RequisitionCreateResponse
              {
                  CreatedRequisitionId = po.Id,
                  TransactionId = po.TransactionId
              });
         }
     }
    
    public class RequisitionSubmit : BaseAsyncEndpoint.WithRequest<RSubmitRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        
        private readonly INotificationService _notificationService;

        public RequisitionSubmit(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _indexAsyncRepository = indexAsyncRepository;
            _notificationService = notificationService;
        }

        [HttpPost("api/requisition/submit")]
        [SwaggerOperation(
            Summary = "Submit Requsition as role Saleman to Accountant",
            Description = "Submit Requsition as role Saleman to Accountant",
            OperationId = "r.submit",
            Tags = new[] { "RequisitionEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(RSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Requisition", UserOperations.Update))
                return Unauthorized();
            
            var po = await _asyncRepository.GetByIdAsync(request.Id);

            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            po.PurchaseOrderStatus = PurchaseOrderStatusType.PQCreated;

            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Purchase Requisition", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok();
        }
    }
    
     public class RequisitionUpdate : BaseAsyncEndpoint.WithRequest<RequisitionUpdateRequest>.WithResponse<RequisitionUpdateResponse>
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
            Description = "Update Requsition as role Saleman ",
            OperationId = "r.update",
            Tags = new[] { "RequisitionEndpoints" })
        ]

        public override async Task<ActionResult<RequisitionUpdateResponse>> HandleAsync(RequisitionUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Requisition", UserOperations.Update))
                return Unauthorized();
                
            var po = await _asyncRepository.GetByIdAsync(request.RequisitionId);
            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;

            foreach (var requestOrderItem in request.OrderItems)
            {
                requestOrderItem.OrderId = po.Id;
                requestOrderItem.ProductVariant = await _pvasyncRepository.GetByIdAsync(requestOrderItem.ProductVariantId);
                requestOrderItem.TotalAmount = requestOrderItem.OrderQuantity * requestOrderItem.Price;
                requestOrderItem.Unit = requestOrderItem.Unit;
            }
            
            po.PurchaseOrderProduct.Clear();
            po.PurchaseOrderProduct = request.OrderItems;
            
            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            po.Deadline = request.Deadline;
            
            
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update","Purchase Requisition", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok(new RequisitionUpdateResponse
            {
                UpdatedRequisitionId = po.Id,
                TransactionId = po.TransactionId
            });
        }
    }

}