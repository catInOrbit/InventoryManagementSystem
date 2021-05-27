using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Data;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint
{
    public class PurchaseOrderCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<PurchaseOrderCreateResponse>
    {
        private readonly UserRoleModificationService _userService;
        private readonly IAsyncRepository<PurchaseOrder> _purchaseRepos;

        
        public PurchaseOrderCreate(UserManager<ApplicationUser> userManager, AppGlobalRepository<UserInfo> userRepository, IAsyncRepository<PurchaseOrder> purchaseRepos)
        {
            _purchaseRepos = purchaseRepos;
            _userService = new UserRoleModificationService( userRepository, userManager);
        }

        
        [HttpPost("api/createpo")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "auth.authenticate",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<PurchaseOrderCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var currentUserId = _userService.UserManager.GetUserId(HttpContext.User);
            var currentUserInfo = await _userService.GetUserInfo(currentUserId);
            var response = new PurchaseOrderCreateResponse();
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder.CreatedById = currentUserId;
            purchaseOrder.CreatedByName = currentUserInfo.Fullname;
            purchaseOrder.DateCreated = DateTime.Now;
            purchaseOrder.purchaseOrderStatus = PurchaseOrderStatus.Draft;
            purchaseOrder.PurchaseOrderProduct = new List<PurchaseOrderProduct>();

            await _purchaseRepos.AddAsync(purchaseOrder);
            return Ok(response);
        }
    }
}