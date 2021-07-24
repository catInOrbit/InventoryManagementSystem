using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints
{
     public class GoodsIssueCreate : BaseAsyncEndpoint.WithRequest<GiCreateRequest>.WithResponse<GiResponse>
        {
            
            private readonly IUserSession _userAuthentication;
            private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
            private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
            private IAsyncRepository<GoodsReceiptOrderItem> _goOrderItemsAsyncRepository;
            private IAsyncRepository<Package> _packageAsyncRepository;
            private readonly IAsyncRepository<GoodsIssueSearchIndex> _goodIssueasyncRepository;

          
            private readonly IAuthorizationService _authorizationService;
            
            private INotificationService _notificationService;

            public GoodsIssueCreate(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<GoodsReceiptOrderItem> goOrderItemsAsyncRepository, IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService, INotificationService notificationService, IAsyncRepository<GoodsIssueSearchIndex> goodIssueasyncRepository)
            {
                _userAuthentication = userAuthentication;
                _asyncRepository = asyncRepository;
                _roAsyncRepository = roAsyncRepository;
                _goOrderItemsAsyncRepository = goOrderItemsAsyncRepository;
                _packageAsyncRepository = packageAsyncRepository;
                _authorizationService = authorizationService;
                _notificationService = notificationService;
                _goodIssueasyncRepository = goodIssueasyncRepository;
            }


            [HttpPost("api/goodsissue/create")]
            [SwaggerOperation(
                Summary = "Create Good issue order",
                Description = "Create Good issue order",
                OperationId = "gio.create",
                Tags = new[] { "GoodsIssueEndpoints" })
            ]
            public override async Task<ActionResult<GiResponse>> HandleAsync(GiCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
            {
                if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSISSUE, UserOperations.Create))
                    return Unauthorized();
                //
                var response = new GiResponse();
                
                var gio = _asyncRepository.GetGoodsIssueOrderByNumber(request.IssueNumber);
                gio.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.GoodsIssue, gio.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
                
                gio.GoodsIssueType = GoodsIssueStatusType.Packing;
                response.GoodsIssueOrder = gio;
                
                //-------------
                ProductStrategyService productStrategyService =
                    new ProductStrategyService(_packageAsyncRepository);
                
                // List<string> productVariantIds = new List<string>();
                // foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
                //     productVariantIds.Add(gioGoodsIssueProduct.ProductVariantId);
                
                var nDictionary = await productStrategyService.FifoPackagesSuggestion(gio.GoodsIssueProducts.ToList());
                
                foreach (var keyValuePair in nDictionary)
                {
                    response.NumOfProductToGetInPackage.Add(new PackageSuggestion
                    {
                        Package = await _packageAsyncRepository.GetByIdAsync(keyValuePair.Key),
                        NumberOfProductToGet = keyValuePair.Value
                    });
                }
                
                await _asyncRepository.UpdateAsync(gio);
                await _goodIssueasyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.GoodsIssueSearchIndexHelper(gio), ElasticIndexConstant.GOODS_ISSUE_ORDERS);

                var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, "Create","Goods Issue", gio.Id);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.STOCKKEEPER,
                    currentUser.Id, messageNotification);
                
                return Ok(response);
            }
        }
     
     public class GoodsIssueUpdate : BaseAsyncEndpoint.WithRequest<GiUpdateRequest>.WithResponse<GiResponse>
     {
         private readonly ILogger<GoodsIssueUpdate> _logger;
        
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;

        private readonly IAsyncRepository<GoodsIssueSearchIndex> _goodIssueasyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private IAsyncRepository<Package> _packageAsyncRepository;

        private INotificationService _notificationService;


        public GoodsIssueUpdate(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, INotificationService notificationService, IAsyncRepository<Package> packageAsyncRepository, IAsyncRepository<GoodsIssueSearchIndex> goodIssueasyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, ILogger<GoodsIssueUpdate> logger)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _packageAsyncRepository = packageAsyncRepository;
            _goodIssueasyncRepository = goodIssueasyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _logger = logger;
        }

        [HttpPut("api/goodsissue/update")]
        [SwaggerOperation(
            Summary = "Update Good issue order",
            Description = "Update Good issue order",
            OperationId = "gio.update",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        public override async Task<ActionResult<GiResponse>> HandleAsync(GiUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
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
            gio.Transaction = TransactionUpdateHelper.UpdateTransaction(gio.Transaction, UserTransactionActionType.Modify,
                (await _userAuthentication.GetCurrentSessionUser()).Id, gio.Id, "");
            
            
            // ProductStrategyService productStrategyService =
            //     new ProductStrategyService(_packageAsyncRepository);
            // var nDictionary = await productStrategyService.FifoPackagesSuggestion(gio.GoodsIssueProducts.ToList());
            //
            // foreach (var productNumPackagePair in nDictionary)
            // {
            //     var listPackages =
            //         await _asyncRepository.GetPackagesFromProductVariantId(productNumPackagePair.Key.Id);
            //     var listPackagesToBeDeleted = new List<Package>();
            //     var productVariant =
            //          await _productVariantAsyncRepository.GetByIdAsync(productNumPackagePair.Key.ProductVariantId);
            //     productVariant.StorageQuantity -= productNumPackagePair.Value;
            //     if (productNumPackagePair.Key.Quantity == productNumPackagePair.Value)
            //         listPackagesToBeDeleted.Add(listPackages.FirstOrDefault(package => package.Id == productNumPackagePair.Key.Id));
            //
            //     else
            //         listPackages.FirstOrDefault(package => package.Id == productNumPackagePair.Key.Id).Quantity -=
            //             productNumPackagePair.Value;
            //     
            //     await _productVariantAsyncRepository.UpdateAsync(productVariant);
            //     foreach (var package in listPackagesToBeDeleted)
            //         await _packageAsyncRepository.DeleteAsync(package);
            // }

            BigQueryService bigQueryService = new BigQueryService();
            var response = new GiResponse();

            if (gio.GoodsIssueType == GoodsIssueStatusType.Shipping)
            {
                foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
                {
                    var listPackages =
                        await _asyncRepository.GetPackagesFromProductVariantId(gioGoodsIssueProduct.ProductVariantId);
                    var productVariant =
                        await _productVariantAsyncRepository.GetByIdAsync(gioGoodsIssueProduct.ProductVariantId);
                    List<int> listIndexPackageToRemove = new List<int>();
                    var quantityToDeduce = gioGoodsIssueProduct.OrderQuantity;
                    for (var i = 0; i < listPackages.Count; i++)
                    {
                        if (quantityToDeduce > 0)
                        {
                            if (listPackages[i].Quantity >= quantityToDeduce)
                            {
                                listPackages[i].Quantity -= quantityToDeduce;
                                listPackages[i].LatestUpdateDate = DateTime.UtcNow;
                                //Remove aggregated quantity of product as well
                                productVariant.StorageQuantity -= quantityToDeduce; 
                            }
                            else
                            {
                                quantityToDeduce -= listPackages[i].Quantity;
                                listPackages[i].Quantity -= listPackages[i].Quantity;
                            
                                //Remove aggregated quantity of product as well
                                productVariant.StorageQuantity -= quantityToDeduce; 
                            }
                        }
            
                        if (listPackages[i].Quantity <= 0)
                            listIndexPackageToRemove.Add(i);
                    }
            
                    await _productVariantAsyncRepository.UpdateAsync(productVariant);

                    foreach (var i in listIndexPackageToRemove)
                    {
                        await _packageAsyncRepository.ElasticDeleteSingleAsync(listPackages[i], ElasticIndexConstant.PACKAGES);
                        await _packageAsyncRepository.DeleteAsync(listPackages[i]);
                    }
                    try
                    {
                        bigQueryService.InsertProductRowBQ(gioGoodsIssueProduct.ProductVariant,
                            gioGoodsIssueProduct.ProductVariant.Price, null,
                            gioGoodsIssueProduct.ProductVariant.StorageQuantity, gioGoodsIssueProduct.OrderQuantity,
                            gioGoodsIssueProduct.SalePrice, "Issue Out", null);
                        _logger.LogInformation("Updated BigQuery on " + this.GetType().ToString());
                    }
                    catch
                    {
                        _logger.LogError("Error updating BigQuery on " + this.GetType().ToString());
                    }
                }
            }

           
            
            await _asyncRepository.UpdateAsync(gio);
            await _goodIssueasyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.GoodsIssueSearchIndexHelper(gio), ElasticIndexConstant.GOODS_ISSUE_ORDERS);

            response.GoodsIssueOrder = gio;
                
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            string messageNoti;
            if (gio.GoodsIssueType == GoodsIssueStatusType.Shipping)
            {
                messageNoti = "Update to Shipping";
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, messageNoti, "Goods Issue", gio.Id);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.ACCOUNTANT,
                    currentUser.Id, messageNotification);
            }
            else
            {
                messageNoti = "Update to Complete";
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, messageNoti, "Goods Issue", gio.Id);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.MANAGER,
                    currentUser.Id, messageNotification);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.ACCOUNTANT,
                    currentUser.Id, messageNotification);
            }
            
            return Ok(response);
        }
    }
     
      public class GoodsIssueCancel : BaseAsyncEndpoint.WithRequest<GiCancel>.WithResponse<GiCancelResponse>
    {
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;

        private readonly IAsyncRepository<GoodsIssueSearchIndex> _goodIssueasyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private IAsyncRepository<Package> _packageAsyncRepository;

        private INotificationService _notificationService;


        public GoodsIssueCancel(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, INotificationService notificationService, IAsyncRepository<Package> packageAsyncRepository, IAsyncRepository<GoodsIssueSearchIndex> goodIssueasyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _packageAsyncRepository = packageAsyncRepository;
            _goodIssueasyncRepository = goodIssueasyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
        }

        [HttpPut("api/goodsissue/reject")]
        [SwaggerOperation(
            Summary = "Reject a goodsisssue",
            Description = "Reject a goodsisssue",
            OperationId = "gio.cancel",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        public override async Task<ActionResult<GiCancelResponse>> HandleAsync(GiCancel request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSISSUE, UserOperations.Update))
                return Unauthorized();
            
            var gio = _asyncRepository.GetGoodsIssueOrderByNumber(request.IssueNumber);
            gio.GoodsIssueType = GoodsIssueStatusType.Cancel;
        
            gio.Transaction = TransactionUpdateHelper.UpdateTransaction(gio.Transaction, UserTransactionActionType.Reject,
                (await _userAuthentication.GetCurrentSessionUser()).Id, gio.Id, request.CancelReason);

            await _asyncRepository.UpdateAsync(gio);
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Cancel", "Goods Issue", gio.Id);
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            
            
            return Ok(new GiCancelResponse()
            {
                GoodsIssueOrder = gio
            });
        }
    }
}