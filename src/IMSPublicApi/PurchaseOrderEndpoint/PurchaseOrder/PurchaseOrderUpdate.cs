using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Data;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
     
    public class PurchaseOrderUpdate : BaseAsyncEndpoint.WithRequest<POUpdateRequest>.WithResponse<POUpdateResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepos;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _poIndexAsyncRepositoryRepos;

        private readonly IUserSession _userAuthentication;
        private readonly IAuthorizationService _authorizationService;

        private readonly INotificationService _notificationService;

        public PurchaseOrderUpdate(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService,  IUserSession userAuthentication, IAsyncRepository<ProductVariant> productVariantRepos, IAsyncRepository<PurchaseOrderSearchIndex> poIndexAsyncRepositoryRepos, INotificationService notificationService)
        {
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productVariantRepos = productVariantRepos;
            _poIndexAsyncRepositoryRepos = poIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
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

            var po =  _purchaseOrderRepos.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            po .Transaction = TransactionUpdateHelper.UpdateTransaction(po.Transaction, UserTransactionActionType.Modify,po.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);
            
            po.PurchaseOrderProduct.Clear();
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
            {
                requestOrderItemInfo.OrderId = po.Id;
                requestOrderItemInfo.ProductVariant = await _productVariantRepos.GetByIdAsync(requestOrderItemInfo.ProductVariantId);
                po.TotalOrderAmount += requestOrderItemInfo.Price;
                po.TotalDiscountAmount += requestOrderItemInfo.DiscountAmount;
                po.PurchaseOrderProduct.Add(requestOrderItemInfo);
                po.MailDescription = request.MailDescription;
            }
      

            await _purchaseOrderRepos.UpdateAsync(po);
            await _poIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false,IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            response.PurchaseOrder = po;
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update","Purchase Order", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            
            return Ok(response);
        }
    }
}