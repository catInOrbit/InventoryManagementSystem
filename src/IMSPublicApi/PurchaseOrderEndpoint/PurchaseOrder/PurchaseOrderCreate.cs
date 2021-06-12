﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
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
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        private readonly IUserAuthentication _userAuthentication;

        public PurchaseOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IUserAuthentication userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository)
        {
            _authorizationService = authorizationService;
            _purchaseOrderRepos = purchaseOrderRepos;
            _userAuthentication = userAuthentication;
            _indexAsyncRepository = indexAsyncRepository;
        }

        public PurchaseOrderCreate(IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository)
        {
            _indexAsyncRepository = indexAsyncRepository;
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


            var poData = _purchaseOrderRepos.GetPurchaseOrderByNumber(request.PurchaseOrderNumber);
            poData.PurchaseOrderStatus = PurchaseOrderStatusType.POCreated;
            // if (poData != null)
            // {
            //     purchaseOrder.PurchaseOrderNumber = poData.PriceQuoteNumber;
            //     purchaseOrder.PurchaseOrderProduct = poData.PurchaseOrderProduct;
            //     purchaseOrder.SupplierId = poData.SupplierId;
            // }

            response.PurchaseOrder = poData;
            await _purchaseOrderRepos.UpdateAsync(poData);
            await _indexAsyncRepository.ElasticSaveSingleAsync(IndexingHelper.PurchaseOrderSearchIndex(poData));
            return Ok(response);
        }
    }
}