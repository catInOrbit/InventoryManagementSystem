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
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderCreate : BaseAsyncEndpoint.WithRequest<PurchaseOrderCreateRequest>.WithResponse<PurchaseOrderCreateResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseRepos;
        private readonly IAsyncRepository<Product> _productRepos;

        public PurchaseOrderCreate(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseRepos, IAsyncRepository<Product> productRepos)
        {
            _purchaseRepos = purchaseRepos;
            _productRepos = productRepos;
        }
        
        [HttpPost("api/createpo")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "auth.authenticate",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<PurchaseOrderCreateResponse>> HandleAsync(PurchaseOrderCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new PurchaseOrderCreateResponse();

            var productList = await _productRepos.ListAllAsync();
            Console.WriteLine(productList.Count);
            await _productRepos.ElasticSaveManyAsync(productList.ToArray());
            
            // await _purchaseRepos.AddAsync(request.PurchaseOrder);
            return Ok(response);
        }
    }
}