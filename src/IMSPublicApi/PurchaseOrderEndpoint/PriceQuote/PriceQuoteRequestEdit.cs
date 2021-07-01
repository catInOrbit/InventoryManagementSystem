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
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
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
            
            po.Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction, UserTransactionActionType.Modify, po.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);
            
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
            
            po.MailDescription = request.MailDescription;
            po.SupplierId = request.SupplierId;
            po.Deadline = request.Deadline;
            
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

            var response = new PQEditResponse();
            response.PriceQuoteResponse = po;
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update", "Price Quote", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            
            return Ok(response);
        }
    }
}