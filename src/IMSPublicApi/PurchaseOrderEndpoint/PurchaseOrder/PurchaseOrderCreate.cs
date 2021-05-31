﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
     
    public class PurchaseOrderCreate : BaseAsyncEndpoint.WithRequest<POCreateRequest>.WithResponse<POCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IUserAuthentication _userAuthentication;

        public PurchaseOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _purchaseOrderRepos = purchaseOrderRepos;
            _userAuthentication = userAuthentication;
        }

        [HttpPost("api/purchaseorder/create")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "po.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<POCreateResponse>> HandleAsync(POCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new POCreateResponse();

            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Create))
                return Unauthorized();

            var purchaseOrder = new ApplicationCore.Entities.Orders.PurchaseOrder();

            var pqData = _purchaseOrderRepos.GetPriceQuoteByNumber(request.PriceQuoteNumber);
            purchaseOrder.CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            purchaseOrder.PurchaseOrderStatus = PurchaseOrderStatusType.Created;
            if (pqData != null)
            {
                purchaseOrder.PurchaseOrderNumber = pqData.PriceQuoteOrderNumber;
                purchaseOrder.PurchaseOrderProduct = pqData.PurchaseOrderProduct;
                purchaseOrder.SupplierId = pqData.SupplierId;
            }

            response.PurchaseOrder = purchaseOrder;
            await _purchaseOrderRepos.AddAsync(purchaseOrder);

            return Ok(response);
        }
    }
}