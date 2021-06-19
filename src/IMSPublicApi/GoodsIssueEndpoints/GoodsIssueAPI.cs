using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints
{
     public class GoodsIssueCreate : BaseAsyncEndpoint.WithRequest<GiRequest>.WithResponse<GiResponse>
        {
            private readonly IUserAuthentication _userAuthentication;
            private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
            private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
            private IAsyncRepository<GoodsReceiptOrderItem> _goOrderItemsAsyncRepository;

            private readonly IAuthorizationService _authorizationService;
    
            public GoodsIssueCreate(IUserAuthentication userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<GoodsReceiptOrderItem> goOrderItemsAsyncRepository)
            {
                _userAuthentication = userAuthentication;
                _asyncRepository = asyncRepository;
                _authorizationService = authorizationService;
                _roAsyncRepository = roAsyncRepository;
                _goOrderItemsAsyncRepository = goOrderItemsAsyncRepository;
            }
            
            [HttpPost("api/goodsissue/create")]
            [SwaggerOperation(
                Summary = "Create Good issue order",
                Description = "Create Good issue order",
                OperationId = "gio.create",
                Tags = new[] { "GoodsIssueEndpoints" })
            ]
            public override async Task<ActionResult<GiResponse>> HandleAsync(GiRequest request, CancellationToken cancellationToken = new CancellationToken())
            {
                if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSISSUE, UserOperations.Create))
                    return Unauthorized();
                
                var response = new GiResponse();
    
                var gio = _asyncRepository.GetGoodsIssueOrderByNumber(request.IssueNumber);
                gio.Transaction = new Transaction
                {
                    CreatedDate = DateTime.Now,
                    Type = TransactionType.PriceQuote,
                    CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                    TransactionStatus = true
                };
                
                gio.GoodsIssueType = GoodsIssueStatusType.Packing;
                response.GoodsIssueOrder = gio;
                
                //-------------
                ProductStrategyService productStrategyService =
                    new ProductStrategyService(_roAsyncRepository, _goOrderItemsAsyncRepository);

                List<string> productIds = new List<string>();
                foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
                    productIds.Add(gioGoodsIssueProduct.ProductVariantId);


                var grs = await productStrategyService.GetFIFOROFromProducts(productIds);
                foreach (var goodsReceiptOrder in grs)
                {
                    response.StrategySuggestions.Add(new GoodsReceiptStrategySuggestion
                    {
                        Location = goodsReceiptOrder.StorageLocationReceipt,
                        GoodsReceiptId = goodsReceiptOrder.Id
                    });
                }
                
                await _asyncRepository.UpdateAsync(gio);
                return Ok(response);
            }
        }
     
     public class GoodsIssueUpdate : BaseAsyncEndpoint.WithRequest<GiRequest>.WithResponse<GiResponse>
    {
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GoodsIssueUpdate(IUserAuthentication userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpPost("api/goodsissue/update")]
        [SwaggerOperation(
            Summary = "Update Good issue order",
            Description = "Update Good issue order",
            OperationId = "gio.update",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        public override async Task<ActionResult<GiResponse>> HandleAsync(GiRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "GoodsIssue", UserOperations.Update))
                return Unauthorized();
            
            var gio = _asyncRepository.GetGoodsIssueOrderByNumber(request.IssueNumber);
            switch (request.ChangeStatusTo)
            {
                case "Shipping":
                    gio.GoodsIssueType = GoodsIssueStatusType.Shipping;
                    break;
                case "Confirm":
                    gio.GoodsIssueType = GoodsIssueStatusType.Completed;
                    break;
            }
            gio.GoodsIssueType = GoodsIssueStatusType.Shipping;
            gio.Transaction.ModifiedDate = DateTime.Now;
            gio.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            await _asyncRepository.UpdateAsync(gio);

            var response = new GiResponse();
            response.GoodsIssueOrder = gio;
            return Ok(response);
        }
    }
}