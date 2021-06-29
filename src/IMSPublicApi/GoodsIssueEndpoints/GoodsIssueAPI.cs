using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints
{
     public class GoodsIssueCreate : BaseAsyncEndpoint.WithRequest<GiRequest>.WithResponse<GiResponse>
        {
            private readonly IUserSession _userAuthentication;
            private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
            private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
            private IAsyncRepository<GoodsReceiptOrderItem> _goOrderItemsAsyncRepository;
            private IAsyncRepository<Package> _packageAsyncRepository;
            
          
            private readonly IAuthorizationService _authorizationService;
            
            private INotificationService _notificationService;

            public GoodsIssueCreate(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<GoodsReceiptOrderItem> goOrderItemsAsyncRepository, IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService, INotificationService notificationService)
            {
                _userAuthentication = userAuthentication;
                _asyncRepository = asyncRepository;
                _roAsyncRepository = roAsyncRepository;
                _goOrderItemsAsyncRepository = goOrderItemsAsyncRepository;
                _packageAsyncRepository = packageAsyncRepository;
                _authorizationService = authorizationService;
                _notificationService = notificationService;
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
                //
                var response = new GiResponse();
                
                var gio = _asyncRepository.GetGoodsIssueOrderByNumber(request.IssueNumber);
                gio.Transaction = new Transaction
                {
                    Name = "Created Goods Issue Order" + gio.Id,
                    CreatedDate = DateTime.Now,
                    Type = TransactionType.GoodsIssue,
                    CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                    TransactionStatus = true
                };
                
                gio.GoodsIssueType = GoodsIssueStatusType.Packing;
                response.GoodsIssueOrder = gio;
                
                //-------------
                ProductStrategyService productStrategyService =
                    new ProductStrategyService(_packageAsyncRepository);
                
                List<string> productVariantIds = new List<string>();
                foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
                    productVariantIds.Add(gioGoodsIssueProduct.ProductVariantId);
                
                var packages = await productStrategyService.FIFOPackagesSuggestion(productVariantIds);
                
                response.Packages.AddRange(packages);
                
                await _asyncRepository.UpdateAsync(gio);

                var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, "Create","Goods Issue", gio.Id);
                
                await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                    currentUser.Id, messageNotification);
                
                return Ok(response);
            }
        }
     
     public class GoodsIssueUpdate : BaseAsyncEndpoint.WithRequest<GiRequest>.WithResponse<GiResponse>
    {
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private IAsyncRepository<Package> _packageAsyncRepository;

        private INotificationService _notificationService;


        public GoodsIssueUpdate(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, INotificationService notificationService, IAsyncRepository<Package> packageAsyncRepository)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _packageAsyncRepository = packageAsyncRepository;
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
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSISSUE, UserOperations.Update))
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
            
            foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
            {
                var listPackages =
                    await _asyncRepository.GetPackagesFromProductVariantId(gioGoodsIssueProduct.ProductVariantId);
                List<int> listIndexPackageToRemove = new List<int>();
                for (var i = 0; i < listPackages.Count; i++)
                {
                    var quantityToDeduceNextPackage = 0;
                    if (listPackages[i].Quantity >= (gioGoodsIssueProduct.OrderQuantity+quantityToDeduceNextPackage))
                    {
                        listPackages[i].Quantity -= gioGoodsIssueProduct.OrderQuantity+quantityToDeduceNextPackage;
                    }
                    else
                    {
                        quantityToDeduceNextPackage = gioGoodsIssueProduct.OrderQuantity - listPackages[i].Quantity;
                        listPackages[i].Quantity -= (gioGoodsIssueProduct.OrderQuantity - quantityToDeduceNextPackage);
                    }
                    
                    if(listPackages[i].Quantity <= 0)
                        listIndexPackageToRemove.Add(i);
                }
                
                foreach (var i in listIndexPackageToRemove)
                    listPackages.RemoveAt(i);
            }
            
            await _asyncRepository.UpdateAsync(gio);

            
            var response = new GiResponse();
            response.GoodsIssueOrder = gio;
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update", "Goods Issue", gio.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            
            return Ok(response);
        }
    }
}