using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Extensions;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
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
            private readonly IElasticAsyncRepository<GoodsIssueSearchIndex> _goodIssueasyncRepository;

          
            private readonly IAuthorizationService _authorizationService;
            
            private INotificationService _notificationService;

            public GoodsIssueCreate(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<GoodsReceiptOrderItem> goOrderItemsAsyncRepository, IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService, INotificationService notificationService, IElasticAsyncRepository<GoodsIssueSearchIndex> goodIssueasyncRepository)
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
                response.IsShowingPackageSuggestion = true;
                
                //-------------
                GoodsIssueBusinessService goodsIssueService =
                    new GoodsIssueBusinessService(_packageAsyncRepository);
                
                // List<string> productVariantIds = new List<string>();
                // foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
                //     productVariantIds.Add(gioGoodsIssueProduct.ProductVariantId);
                
                response.ProductPackageFIFO = await goodsIssueService.FifoPackageCalculate(gio.GoodsIssueProducts.ToList());
                
                // foreach (var keyValuePair in nDictionary)
                // {
                //     
                //     response.ProductPackageFIFO.Add(new FifoPackageSuggestion
                //     {
                //         Packages = await _packageAsyncRepository.GetByIdAsync(keyValuePair.Key),
                //         NumberOfProductToGet = keyValuePair.Value
                //     });
                // }
                
                await _asyncRepository.UpdateAsync(gio);
                await _goodIssueasyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.GoodsIssueSearchIndexHelper(gio), ElasticIndexConstant.GOODS_ISSUE_ORDERS);

                var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, "Create","Goods Issue", gio.Id);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.STOCKKEEPER,
                    currentUser.Id, messageNotification, PageConstant.GOODSISSUE, gio.Id);
                
                return Ok(response);
            }
        }
     
     public class GoodsIssueDEMO : BaseAsyncEndpoint.WithRequest<GiDEMORequest>.WithoutResponse
        {
            
            private readonly IUserSession _userAuthentication;
            private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
            private readonly IAsyncRepository<Supplier> _supAsyncRepository;
            private readonly IAsyncRepository<ProductVariant> _pvAsyncRepository;

            private readonly IElasticAsyncRepository<GoodsIssueSearchIndex> _giEls;

          
            private readonly IAuthorizationService _authorizationService;
            

            public GoodsIssueDEMO(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAsyncRepository<Supplier> supAsyncRepository, IElasticAsyncRepository<GoodsIssueSearchIndex> giEls, IAuthorizationService authorizationService, IAsyncRepository<ProductVariant> pvAsyncRepository)
            {
                _userAuthentication = userAuthentication;
                _asyncRepository = asyncRepository;
                _supAsyncRepository = supAsyncRepository;
                _giEls = giEls;
                _authorizationService = authorizationService;
                _pvAsyncRepository = pvAsyncRepository;
            }


            [HttpPost("api/goodsissue/DEMO")]
            [SwaggerOperation(
                Summary = "Create Good issue order",
                Description = "Create Good issue order",
                OperationId = "gio.demo",
                Tags = new[] { "GoodsIssueEndpoints" })
            ]
            public override async Task<ActionResult> HandleAsync(GiDEMORequest request, CancellationToken cancellationToken = new CancellationToken())
            {
                if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSISSUE, UserOperations.Create))
                    return Unauthorized();

                if (request.IsAutoGenerated)
                {
                        var random = new Random();
                    var supList = (await _supAsyncRepository.ListAllAsync(new PagingOption<Supplier>(0, 0))).ResultList
                        .ToList();
                    var randomSupplier = supList[random.Next(supList.Count)]; 
                    var gio = new GoodsIssueOrder
                    {
                        Supplier = randomSupplier,
                        SupplierId = randomSupplier.Id,
                        GoodsIssueType = GoodsIssueStatusType.IssueRequisition,
                        CustomerName = "JakeA",
                        CustomerPhoneNumber = "12345678",
                        DeliveryAddress = "Delivery Address",
                        DeliveryDate = DateTime.Now.AddDays(10),
                        DeliveryMethod = "Fast Shipping"
                    };

                    gio.GoodsIssueProducts = new List<OrderItem>();
                    var variantList = (await _pvAsyncRepository.ListAllAsync(new PagingOption<ProductVariant>(0, 0)))
                        .ResultList;

                    var randomList = new List<ProductVariant>();

                    var randomNumOfOrders = random.Next(1, 5);
                    List<string> variantIdList = new List<string>();
                    while (randomList.Count <= randomNumOfOrders)
                    {
                        var randomVariant =  variantList[random.Next(variantList.Count)];

                        if (!variantIdList.Contains(randomVariant.Id) && randomVariant.Packages.Count > 0 && randomVariant.StorageQuantity > 0)
                        {
                            variantIdList.Add(randomVariant.Id);
                            randomList.Add(randomVariant);
                        }
                    }
             
                    
                    foreach (var productVariant in randomList)
                    {
                        var currentProductQuantity = productVariant.StorageQuantity;
                        var orderItem = new OrderItem
                        {
                            ProductVariant = productVariant,
                            ProductVariantId = productVariant.Id,
                            Unit = "UnitDemo",
                            DiscountAmount = 0,
                            OrderQuantity = random.Next(1, currentProductQuantity),
                            Price = random.Next(1, 100000),
                            SalePrice = random.Next(1, 100000),
                        };
                        orderItem.TotalAmount = orderItem.Price * orderItem.OrderQuantity;
                        orderItem.QuantityLeftAfterReceived = orderItem.OrderQuantity;
                        gio.GoodsIssueProducts.Add(orderItem);
                    }
                    
                    gio.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.GoodsIssue, gio.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
                    
                    gio.GoodsIssueType = GoodsIssueStatusType.IssueRequisition;

                    await _asyncRepository.AddAsync(gio);
                    await _giEls.ElasticSaveSingleAsync(false, IndexingHelper.GoodsIssueSearchIndexHelper(gio), ElasticIndexConstant.GOODS_ISSUE_ORDERS);

                    return Ok(gio.Id);
                }

                else
                {
                    GoodsIssueOrder gio = null;
                    try
                    {
                        gio = new GoodsIssueOrder
                        {
                            Supplier = await _supAsyncRepository.GetByIdAsync(request.GoodsIssueOrderRequest.SupplierId),
                            SupplierId = request.GoodsIssueOrderRequest.SupplierId,
                            GoodsIssueType = GoodsIssueStatusType.IssueRequisition,
                            CustomerName = request.GoodsIssueOrderRequest.CustomerName,
                            CustomerPhoneNumber = request.GoodsIssueOrderRequest.CustomerPhoneNumber,
                            DeliveryAddress = request.GoodsIssueOrderRequest.DeliveryAddress,
                            DeliveryDate = request.GoodsIssueOrderRequest.DeliveryDate,
                            DeliveryMethod = request.GoodsIssueOrderRequest.DeliveryMethod,
                            GoodsIssueProducts = new List<OrderItem>()
                        };

                        foreach (var gioGoodsIssueProduct in request.GoodsIssueOrderRequest.GoodsIssueProducts)
                        {
                            gioGoodsIssueProduct.ProductVariant = await 
                                _pvAsyncRepository.GetByIdAsync(gioGoodsIssueProduct.ProductVariantId);
                            gioGoodsIssueProduct.TotalAmount = gioGoodsIssueProduct.Price * gioGoodsIssueProduct.OrderQuantity;
                            gioGoodsIssueProduct.QuantityLeftAfterReceived = gioGoodsIssueProduct.OrderQuantity;
                            gio.GoodsIssueProducts.Add(gioGoodsIssueProduct);
                        }
                        
                        gio.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.GoodsIssue, gio.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
                    
                        gio.GoodsIssueType = GoodsIssueStatusType.IssueRequisition;

                        await _asyncRepository.AddAsync(gio);
                        await _giEls.ElasticSaveSingleAsync(false, IndexingHelper.GoodsIssueSearchIndexHelper(gio), ElasticIndexConstant.GOODS_ISSUE_ORDERS);
                    }
                    catch (Exception e)
                    {
                        return BadRequest("Incorrect input data");
                    }
                  
                    return Ok(gio.Id);
                }
                
            }
        }
     public class GoodsIssueUpdate : BaseAsyncEndpoint.WithRequest<GiUpdateRequest>.WithResponse<GiResponse>
     {
         private readonly ILogger<GoodsIssueUpdate> _logger;
        
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;
         
        private readonly IElasticAsyncRepository<GoodsIssueSearchIndex> _goodIssueasyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private IAsyncRepository<Package> _packageAsyncRepository;
        private IElasticAsyncRepository<Package> _packageIndexAsyncRepository;


        private INotificationService _notificationService;


        public GoodsIssueUpdate(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, INotificationService notificationService, IAsyncRepository<Package> packageAsyncRepository, IElasticAsyncRepository<GoodsIssueSearchIndex> goodIssueasyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, ILogger<GoodsIssueUpdate> logger, IElasticAsyncRepository<Package> packageIndexAsyncRepository)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _packageAsyncRepository = packageAsyncRepository;
            _goodIssueasyncRepository = goodIssueasyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _logger = logger;
            _packageIndexAsyncRepository = packageIndexAsyncRepository;
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
            if (gio == null)
                return NotFound("Can not find GoodsIssue of id :" + request.IssueNumber);
            
            switch (request.ChangeStatusTo)
            {
                case "Shipping":
                    gio.GoodsIssueType = GoodsIssueStatusType.Shipping;
                    break;
                case "Confirm":
                    gio.GoodsIssueType = GoodsIssueStatusType.Completed;
                    break;
            }
            gio.Transaction = TransactionUpdateHelper.UpdateTransaction(gio.Transaction, UserTransactionActionType.Modify,TransactionType.GoodsIssue,
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
            response.IsShowingPackageSuggestion = false;
            GoodsIssueBusinessService gis = new GoodsIssueBusinessService(_asyncRepository, _productVariantAsyncRepository,
                _packageAsyncRepository, _packageIndexAsyncRepository);
            if (gio.GoodsIssueType == GoodsIssueStatusType.Shipping)
            {
                var error = gis.ValidateGoodsIssue(gio);
                if (error != null)
                    return BadRequest(error);
                
                await gis.UpdatePackageFromGoodsIssue(gio, (await _userAuthentication.GetCurrentSessionUser()).Id);
            }

            foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
            {
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
                    currentUser.Id, messageNotification, PageConstant.GOODSISSUE, gio.Id);
            }
            else
            {
                messageNoti = "Update to Complete";
                var messageNotification =
                    _notificationService.CreateMessage(currentUser.Fullname, messageNoti, "Goods Issue", gio.Id);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.MANAGER,
                    currentUser.Id, messageNotification,PageConstant.GOODSISSUE, gio.Id);
                
                await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.ACCOUNTANT,
                    currentUser.Id, messageNotification,PageConstant.GOODSISSUE, gio.Id);
            }
            
            return Ok(response);
        }
    }
     
      public class GoodsIssueCancel : BaseAsyncEndpoint.WithRequest<GiCancel>.WithResponse<GiCancelResponse>
    {
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;
        private readonly IElasticAsyncRepository<GoodsIssueSearchIndex> _goodIssueElasticAsyncRepository;

        private readonly IAsyncRepository<GoodsIssueSearchIndex> _goodIssueasyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private IAsyncRepository<Package> _packageAsyncRepository;

        private INotificationService _notificationService;


        public GoodsIssueCancel(IUserSession userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, INotificationService notificationService, IAsyncRepository<Package> packageAsyncRepository, IAsyncRepository<GoodsIssueSearchIndex> goodIssueasyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, IElasticAsyncRepository<GoodsIssueSearchIndex> goodIssueElasticAsyncRepository)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _packageAsyncRepository = packageAsyncRepository;
            _goodIssueasyncRepository = goodIssueasyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _goodIssueElasticAsyncRepository = goodIssueElasticAsyncRepository;
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
            if (gio == null)
                return NotFound("Can not find GoodsIssue of id :" + request.IssueNumber);
            
            gio.GoodsIssueType = GoodsIssueStatusType.Cancel;
            await _goodIssueElasticAsyncRepository.ElasticSaveSingleAsync(false,
                IndexingHelper.GoodsIssueSearchIndexHelper(gio), ElasticIndexConstant.GOODS_ISSUE_ORDERS);
        
            gio.Transaction = TransactionUpdateHelper.UpdateTransaction(gio.Transaction, UserTransactionActionType.Reject,TransactionType.GoodsIssue,
                (await _userAuthentication.GetCurrentSessionUser()).Id, gio.Id, request.CancelReason);

            await _asyncRepository.UpdateAsync(gio);
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Cancel", "Goods Issue", gio.Id);
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification, PageConstant.GOODSISSUE, gio.Id);
            
            
            return Ok(new GiCancelResponse()
            {
                GoodsIssueOrder = gio
            });
        }
    }
}