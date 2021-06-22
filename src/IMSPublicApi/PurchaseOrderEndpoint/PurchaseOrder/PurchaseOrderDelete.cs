﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderDelete : BaseAsyncEndpoint.WithRequest<PODeleteRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _poSearchRepos;
        
        private readonly INotificationService _notificationService;
        public PurchaseOrderDelete(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> poSearchRepos, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _poSearchRepos = poSearchRepos;
            _notificationService = notificationService;
        }
        
        [HttpPut("api/purchaseorder/cancel/{Id}")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "po.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] PODeleteRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Delete))
                return Unauthorized();

            var po = await _asyncRepository.GetByIdAsync(request.Id);
            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            po.Transaction.TransactionStatus = false;
            po.PurchaseOrderStatus = PurchaseOrderStatusType.POCanceled;
           await _asyncRepository.UpdateAsync(po,cancellationToken);
           await _poSearchRepos.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
           
           var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
           var messageNotification =
               _notificationService.CreateMessage(currentUser.Fullname, "Cancel","Purchase Order", po.Id);
                
           await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
               currentUser.Id, messageNotification);

           return Ok();
        }
    }
}