﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderUpdateItem : BaseAsyncEndpoint.WithRequest<ROUpdateItemRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _recevingOrderRepository;
        private readonly IAsyncRepository<GoodsReceiptOrderSearchIndex> _recevingOrderSearchIndexRepository;

        private readonly IAsyncRepository<ProductVariant> _productRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderUpdateItem(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> recevingOrderRepository, IAsyncRepository<ProductVariant> productRepository, IUserAuthentication userAuthentication, IAsyncRepository<GoodsReceiptOrderSearchIndex> recevingOrderSearchIndexRepository)
        {
            _authorizationService = authorizationService;
            _recevingOrderRepository = recevingOrderRepository;
            _productRepository = productRepository;
            _userAuthentication = userAuthentication;
            _recevingOrderSearchIndexRepository = recevingOrderSearchIndexRepository;
        }

        
        [HttpPost("api/goodsreceipt/adjust")]
        [SwaggerOperation(
            Summary = "Update Receiving Order",
            Description = "Update Receiving Order",
            OperationId = "gr.updateitem",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult> HandleAsync(ROUpdateItemRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
        
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            var ro = _recevingOrderRepository.GetReceivingOrderByNumber(request.CurrentReceivingOrderNumber);
            // var productVariant = await _productRepository.GetByIdAsync(request.ProductVariantId);
            
            //Transaction Update
            ro.Transaction.ModifiedDate = DateTime.Now;
            ro.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;

            //Update item row in order 
            foreach (var goodsReceiptOrderItem in request.UpdateItems)
            {
                var productVariant = await _productRepository.GetByIdAsync(goodsReceiptOrderItem.ProductVariantId);
                productVariant.StorageQuantity = goodsReceiptOrderItem.QuantityReceived;
            }
            
            //Update and indexing
            await _recevingOrderRepository.UpdateAsync(ro);
            await _recevingOrderSearchIndexRepository.ElasticSaveSingleAsync(false,IndexingHelper.GoodsReceiptOrderSearchIndex(ro));
            return Ok();
        }
    }
}