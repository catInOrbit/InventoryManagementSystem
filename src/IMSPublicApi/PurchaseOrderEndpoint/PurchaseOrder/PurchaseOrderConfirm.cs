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
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderConfirm : BaseAsyncEndpoint.WithRequest<POConfirmRequest>.WithoutResponse
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _poSearchRepos;
        private readonly IUserSession _userAuthentication;


        private readonly IAuthorizationService _authorizationService;
        
        private INotificationService _notificationService;

        public PurchaseOrderConfirm(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService, IAsyncRepository<PurchaseOrderSearchIndex> poSearchRepos, INotificationService notificationService, IUserSession userAuthentication)
        {
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
            _poSearchRepos = poSearchRepos;
            _notificationService = notificationService;
            _userAuthentication = userAuthentication;
        }

        [HttpPost("api/po/confirm/{PurchaseOrderNumber}")]
        [SwaggerOperation(
            Summary = "Confirm purchase order ",
            Description = "Confirm purchase order",
            OperationId = "catalog-items.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute]POConfirmRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Approve))
                return Unauthorized();
            
            var po = _purchaseOrderRepos.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            po.PurchaseOrderStatus = PurchaseOrderStatusType.POConfirm;
            await _purchaseOrderRepos.UpdateAsync(po);
            await _poSearchRepos.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction,UserTransactionActionType.Confirm,
                (await _userAuthentication.GetCurrentSessionUser()).Id, po.Id, "");
            
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Confirm","Purchase Order", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            return Ok();
        }
    }
}