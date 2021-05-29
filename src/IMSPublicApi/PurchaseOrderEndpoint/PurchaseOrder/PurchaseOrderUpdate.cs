using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Data;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
     
    public class PurchaseOrderUpdate : BaseAsyncEndpoint.WithRequest<PurchaseOrderUpdateRequest>.WithResponse<PurchaseOrderUpdateResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IAsyncRepository<Product> _productRepos;
        private readonly IAsyncRepository<Supplier> _supplierRepos;
        private readonly IAuthorizationService _authorizationService;

        public PurchaseOrderUpdate(IAsyncRepository<Product> productRepos, IAsyncRepository<Supplier> supplierRepos, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService)
        {
            _productRepos = productRepos;
            _supplierRepos = supplierRepos;
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
        }
        
        [HttpPut("api/updatepo")]
        [SwaggerOperation(
            Summary = "Update purchase order",
            Description = "Update purchase order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<PurchaseOrderUpdateResponse>> HandleAsync(PurchaseOrderUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new PurchaseOrderUpdateResponse();

            // requires using ContactManager.Authorization;
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                response.Result = false;
                response.Verbose = "Not authorized as privilege user";
                return Unauthorized(response);
            }

            var productList = await _productRepos.ListAllAsync();
            // var supplierList = await _supplierRepos.ListAllAsync();
            
            await _productRepos.ElasticSaveManyAsync(productList.ToArray());
            // await _supplierRepos.ElasticSaveManyAsync(supplierList.ToArray());

            await _purchaseOrderRepos.AddAsync(request.PurchaseOrder);  
            return Ok(response);
        }
    }
}