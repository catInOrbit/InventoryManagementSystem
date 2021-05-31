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
     
    public class PurchaseOrderUpdate : BaseAsyncEndpoint.WithRequest<POUpdateRequest>.WithResponse<POUpdateResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IAsyncRepository<Product> _productRepos;

        private readonly IUserAuthentication _userAuthentication;
        private readonly IAuthorizationService _authorizationService;

        public PurchaseOrderUpdate(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IAuthorizationService authorizationService,  IUserAuthentication userAuthentication, IAsyncRepository<Product> productRepos)
        {
            _purchaseOrderRepos = purchaseOrderRepos;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productRepos = productRepos;
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
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Update))
                return Unauthorized();


            var po =  _purchaseOrderRepos.GetPurchaseOrderByNumber(request.PurchaseOrderNumberGet);
            po.ModifiedDate = DateTime.Now;
            po.ModifiedBy = (await _userAuthentication.GetCurrentSessionUser()).Id;
            po.PurchaseOrderStatus = PurchaseOrderStatusType.WaitingConfirmation;
            
            foreach (var requestOrderItemInfo in request.OrderItemInfos)
            {
                requestOrderItemInfo.OrderNumber = po.PurchaseOrderNumber;
                requestOrderItemInfo.Product = await _productRepos.GetByIdAsync(requestOrderItemInfo.ProductId);
                requestOrderItemInfo.TotalAmount += requestOrderItemInfo.Price;  
                po.PurchaseOrderProduct.Add(requestOrderItemInfo);
            }
            // var supplierList = await _supplierRepos.ListAllAsync();
            // var indexList = productList.Select(p => new {p.Id, p.Name});
            // await _supplierRepos.ElasticSaveManyAsync(supplierList.ToArray());

            await _purchaseOrderRepos.UpdateAsync(po);
            response.PurchaseOrder = po;
            return Ok(response);
        }
    }
}