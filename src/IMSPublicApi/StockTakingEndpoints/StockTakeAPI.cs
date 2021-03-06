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
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints
{
    public class StockTakeSubmit : BaseAsyncEndpoint.WithRequest<StockTakeSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<StockTakeOrder> _asyncRepository;
        private IElasticAsyncRepository<StockTakeSearchIndex> _stSearchasyncRepository;

        private IAsyncRepository<ProductVariant> _poAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly IUserSession _userAuthentication;

        public StockTakeSubmit(IAsyncRepository<StockTakeOrder> asyncRepository, IAuthorizationService authorizationService, IAsyncRepository<ProductVariant> poAsyncRepository, INotificationService notificationService, IUserSession userAuthentication,  IElasticAsyncRepository<StockTakeSearchIndex> stSearchasyncRepository1)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _poAsyncRepository = poAsyncRepository;
            _notificationService = notificationService;
            _userAuthentication = userAuthentication;
            _stSearchasyncRepository = stSearchasyncRepository1;
        }

        [HttpPut("api/stocktake/submit")]
        [SwaggerOperation(
            Summary = "Submit and complete a stocktake",
            Description = "Submit and complete a stocktake",
            OperationId = "st.submit",
            Tags = new[] { "StockTakingEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(StockTakeSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            var stockTakeOrder = await _asyncRepository.GetByIdAsync(request.Id);
            if (stockTakeOrder == null)
                return NotFound("Can not find StockTake of id :" + request.Id);
                
            if (!await UserAuthorizationService.AuthorizeWithUserId(_authorizationService,currentUser.Id, stockTakeOrder.Transaction.TransactionRecord[^1].ApplicationUserId, HttpContext.User,
                PageConstant.STOCKTAKEORDER, UserOperations.Update))
                return Unauthorized();
            
            stockTakeOrder.StockTakeOrderType = StockTakeOrderType.AwaitingAdjustment;
            stockTakeOrder.Transaction = TransactionUpdateHelper.UpdateTransaction(stockTakeOrder.Transaction,UserTransactionActionType.Submit,TransactionType.StockTake,
                (await _userAuthentication.GetCurrentSessionUser()).Id, stockTakeOrder.Id, "");
            
            
            await _asyncRepository.UpdateAsync(stockTakeOrder);
            await _stSearchasyncRepository.ElasticSaveSingleAsync(false,
                IndexingHelper.StockTakeSearchIndex(stockTakeOrder), ElasticIndexConstant.STOCK_TAKE_ORDERS);
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Stock Take", stockTakeOrder.Id);
                
            await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.MANAGER,
                currentUser.Id, messageNotification, PageConstant.STOCKTAKEORDER, stockTakeOrder.Id);

            return Ok();
        }
    }
    
     public class StockTakeUpdate : BaseAsyncEndpoint.WithRequest<STSingleUpdateRequest>.WithResponse<STUpdateResponse>
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _pvasyncRepository;
        private IElasticAsyncRepository<StockTakeSearchIndex> _stSearchasyncRepository;
        private IAsyncRepository<Package> _packageAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;

        public StockTakeUpdate(IAsyncRepository<StockTakeOrder> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductVariant> pvasyncRepository, INotificationService notificationService, IElasticAsyncRepository<StockTakeSearchIndex> stSearchasyncRepository1, IAsyncRepository<Package> packageAsyncRepository)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _pvasyncRepository = pvasyncRepository;
            _notificationService = notificationService;
            _stSearchasyncRepository = stSearchasyncRepository1;
            _packageAsyncRepository = packageAsyncRepository;
        }

        [HttpPut("api/stocktake/updatesingle")]
        [SwaggerOperation(
            Summary = "Update a single line of a stock take",
            Description = "Update a single line of a stock take",
            OperationId = "st.updatesingle",
            Tags = new[] { "StockTakingEndpoints" })
        ]

        public override async Task<ActionResult<STUpdateResponse>> HandleAsync(STSingleUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Update))
                return Unauthorized();

            //TODO: Fix Error
            var response = new STUpdateResponse();

            var storder = await _asyncRepository.GetByIdAsync(request.StockTakeId);
            
            foreach (var groupLocation in storder.GroupLocations)
            {
                if (groupLocation.CheckItems.FirstOrDefault(st => st.Id == request.StockTakeItemId) != null)
                {
                    var storderCheckItem = groupLocation.CheckItems.FirstOrDefault(st => st.Id == request.StockTakeItemId);
                    storderCheckItem.Note = request.Note;
                    storderCheckItem.ActualQuantity = request.ActualQuantity;

                    var packageGet = await _packageAsyncRepository.GetByIdAsync(storderCheckItem.PkgId);
                    var productVariant = await _pvasyncRepository.GetByIdAsync(packageGet.ProductVariantId);
                    
                    var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);
                    Console.WriteLine("Number of packages found: " + packages.Count);

                    foreach (var package in packages)
                    {
                        if (request.ActualQuantity != package.Quantity)
                            response.MismatchQuantityPackageIds.Add(package.Id);
                    }
                }
            }
            
            storder.StockTakeOrderType = StockTakeOrderType.Progressing;
            storder.Transaction = TransactionUpdateHelper.UpdateTransaction(storder.Transaction,UserTransactionActionType.Modify,TransactionType.StockTake,
                (await _userAuthentication.GetCurrentSessionUser()).Id, storder.Id, "");
            await _asyncRepository.UpdateAsync(storder);
            await _stSearchasyncRepository.ElasticSaveSingleAsync(false,
                IndexingHelper.StockTakeSearchIndex(storder), ElasticIndexConstant.STOCK_TAKE_ORDERS);
   
            return Ok(response);
        }
    }
     
     public class StockTakeAddProduct : BaseAsyncEndpoint.WithRequest<STAddRequest>.WithResponse<STCreateItemResponse>
     {
         private readonly IAsyncRepository<StockTakeOrder> _stAsyncRepository;
         private readonly IElasticAsyncRepository<StockTakeSearchIndex> _stSearchAsyncRepository;

         private readonly IAsyncRepository<ProductVariant> _productAsyncRepository;
         private readonly IAsyncRepository<Location> _locationAsyncRepository;
         private readonly IAsyncRepository<Package> _packageAsyncRepository;
         private readonly IAuthorizationService _authorizationService;

         private readonly IUserSession _userAuthentication;
         private readonly INotificationService _notificationService;

         private readonly IRedisRepository _redisRepository;

         public StockTakeAddProduct(IAsyncRepository<StockTakeOrder> stAsyncRepository, IAsyncRepository<ProductVariant> productAsyncRepository, IAsyncRepository<Location> locationAsyncRepository, IUserSession userAuthentication, INotificationService notificationService, IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService, IElasticAsyncRepository<StockTakeSearchIndex> stSearchAsyncRepository1, IRedisRepository redisRepository)
         {
             _stAsyncRepository = stAsyncRepository;
             _productAsyncRepository = productAsyncRepository;
             _locationAsyncRepository = locationAsyncRepository;
             _userAuthentication = userAuthentication;
             _notificationService = notificationService;
             _packageAsyncRepository = packageAsyncRepository;
             _authorizationService = authorizationService;
             _stSearchAsyncRepository = stSearchAsyncRepository1;
             _redisRepository = redisRepository;
         }


         [HttpPut("api/stocktake/add")]
         [SwaggerOperation(
             Summary = "Add product to check to stock take order",
             Description = "Add product to check to stock take order",
             OperationId = "st.add",
             Tags = new[] { "StockTakingEndpoints" })
         ]
         public override async Task<ActionResult<STCreateItemResponse>> HandleAsync(STAddRequest request, CancellationToken cancellationToken = new CancellationToken())
         {
             if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Update))
                 return Unauthorized();
             
             var currentUser = await _userAuthentication.GetCurrentSessionUser();
             var response = new STCreateItemResponse();
             StockTakeOrder stockTakeOrder = null;
             bool isNewStockTake = false;
             if (request.StockTakeId == null)
             {
                 stockTakeOrder = new StockTakeOrder();
                 isNewStockTake = true;
             }

             else
             {
                 stockTakeOrder= await _stAsyncRepository.GetByIdAsync(request.StockTakeId);

                 if (!await UserAuthorizationService.AuthorizeWithUserId(_authorizationService,currentUser.Id, stockTakeOrder.Transaction.TransactionRecord[^1].ApplicationUserId, HttpContext.User,
                     PageConstant.STOCKTAKEORDER, UserOperations.Update))
                     return Unauthorized();
             }
             
             stockTakeOrder.GroupLocations.Clear();
             
             foreach (var stockTakeGroupLocation in request.StockTakeGroupLocation)
             {
                 var location = await _locationAsyncRepository.GetByIdAsync(stockTakeGroupLocation.LocationId);
                 if (location == null)
                 {
                     response.Status = false;
                     response.Verbose = "Location does not exist in database";
                     return NotFound(response);
                 }
                 
                 stockTakeOrder.GroupLocations.Add(stockTakeGroupLocation);
             }
             
             foreach (var stockTakeGroupLocation in stockTakeOrder.GroupLocations)
             {
                 foreach (var stockTakeItem in stockTakeGroupLocation.CheckItems)
                 {
                     var package = await _packageAsyncRepository.GetByIdAsync(stockTakeItem.PkgId);
                     if (stockTakeItem.ActualQuantity != package.Quantity)
                         response.MismatchQuantityPackageIds.Add(package.Id);
                     stockTakeItem.StockTakeOrderId = stockTakeOrder.Id;
                     stockTakeItem.IsShowingPackageId = true;

                     await _redisRepository.AddStockTakeAdjustMessage(new StockTakeAdjustItemInfo()
                     {
                         PackageId = stockTakeItem.PkgId,
                         QuantityToAdjust = stockTakeItem.ActualQuantity,
                         StockTakeId = stockTakeOrder.Id,
                         StockTakeItemId = stockTakeItem.Id
                     });
                 }
             }
             
             stockTakeOrder.StockTakeOrderType = StockTakeOrderType.Progressing;
             
             if(isNewStockTake)
                 stockTakeOrder.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.StockTake,stockTakeOrder.Id,
                     (await _userAuthentication.GetCurrentSessionUser()).Id);
             else
                 stockTakeOrder.Transaction = TransactionUpdateHelper.UpdateTransaction(stockTakeOrder.Transaction,UserTransactionActionType.Modify,TransactionType.StockTake,
                     (await _userAuthentication.GetCurrentSessionUser()).Id, stockTakeOrder.Id, "");
             try
             {
                 if(isNewStockTake)
                     await _stAsyncRepository.AddAsync(stockTakeOrder);
                 else
                     await _stAsyncRepository.UpdateAsync(stockTakeOrder);

                 await _stSearchAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.StockTakeSearchIndex(stockTakeOrder),
                     ElasticIndexConstant.STOCK_TAKE_ORDERS);
             }
             catch
             {
                 response.Status = false;
                 response.Verbose = "Fail to update stocktake, request may be wrong";
                 return NotFound(response);
             }

             response.Status = true;
             response.Verbose = "Update Stock Take Order";
             response.StockTakeOrder = stockTakeOrder;
             response.StockTakeOrderId = stockTakeOrder.Id;
            
             return Ok(response);
         }
     }
     
      public class StockTakeAdjust : BaseAsyncEndpoint.WithRequest<STAdjustRequest>.WithResponse<STResponse>
     {
         private readonly IAsyncRepository<StockTakeOrder> _stAsyncRepository;
         private readonly IElasticAsyncRepository<StockTakeSearchIndex> _stSearchAsyncRepository;

         private readonly IAsyncRepository<ProductVariant> _productAsyncRepository;
         private readonly IAsyncRepository<Location> _locationAsyncRepository;
         private readonly IAsyncRepository<Package> _packageAsyncRepository;
         private readonly IAuthorizationService _authorizationService;

         private readonly IUserSession _userAuthentication;
         private readonly IRedisRepository _redisRepository;
         private readonly INotificationService _notificationService;

         public StockTakeAdjust(IAsyncRepository<StockTakeOrder> stAsyncRepository, IAsyncRepository<ProductVariant> productAsyncRepository, IAsyncRepository<Location> locationAsyncRepository, IUserSession userAuthentication, INotificationService notificationService, IAsyncRepository<Package> packageAsyncRepository, IAuthorizationService authorizationService, IElasticAsyncRepository<StockTakeSearchIndex> stSearchAsyncRepository1, IRedisRepository redisRepository)
         {
             _stAsyncRepository = stAsyncRepository;
             _productAsyncRepository = productAsyncRepository;
             _locationAsyncRepository = locationAsyncRepository;
             _userAuthentication = userAuthentication;
             _notificationService = notificationService;
             _packageAsyncRepository = packageAsyncRepository;
             _authorizationService = authorizationService;
             _stSearchAsyncRepository = stSearchAsyncRepository1;
             _redisRepository = redisRepository;
         }
         [HttpPost("api/stocktake/adjust")]
         [SwaggerOperation(
             Summary = "Add product to check to stock take order",
             Description = "Add product to check to stock take order",
             OperationId = "st.adjust",
             Tags = new[] { "StockTakingEndpoints" })
         ]
         public override async Task<ActionResult<STResponse>> HandleAsync(STAdjustRequest request, CancellationToken cancellationToken = new CancellationToken())
         {
             if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Update))
                 return Unauthorized();
             
             Dictionary<ProductVariant, int> quantityUpdateProductDict = new Dictionary<ProductVariant, int>();
             var stockTakeOrder = await _stAsyncRepository.GetByIdAsync(request.StockTakeId);
             if (stockTakeOrder == null)
                 return NotFound("Can not find StockTake of id :" + request.StockTakeId);
             
             var stockTakeMessageGroup = await _redisRepository.GetStockTakeAdjustMessage();
             var stockTakeAdjustInfo = await _redisRepository.GetStockTakeAdjustInfo();
             // foreach (var stockTakeGroupLocation in stockTakeOrder.GroupLocations)
             // {
             //     foreach (var stockTakeItem in stockTakeGroupLocation.CheckItems)
             //     {
             //         var package = await _packageAsyncRepository.GetByIdAsync(stockTakeItem.PackageId);
             //         package.Quantity = stockTakeItem.ActualQuantity;
             //         
             //         var productVariant = package.ProductVariant;
             //         
             //         // productVariant.StorageQuantity += package.Quantity;
             //         
             //         if (quantityUpdateProductDict.ContainsKey(productVariant))
             //             quantityUpdateProductDict[productVariant] += package.Quantity;
             //         else
             //             quantityUpdateProductDict.Add(productVariant, package.Quantity);
             //     }
             // }

             List<StockTakeAdjustItemInfo> deleteIndex = new List<StockTakeAdjustItemInfo>();
             for (var i = 0; i < stockTakeAdjustInfo.StockTakeAdjustItemsInfos.Count; i++)
             {
                 if (stockTakeAdjustInfo.StockTakeAdjustItemsInfos[i].StockTakeId == stockTakeOrder.Id)
                 {
                     var package = await _packageAsyncRepository.GetByIdAsync(stockTakeAdjustInfo.StockTakeAdjustItemsInfos[i].PackageId);
                     package.Quantity = stockTakeAdjustInfo.StockTakeAdjustItemsInfos[i].QuantityToAdjust;
                     package.TotalPrice = package.Price * package.Quantity;
                     var productVariant = package.ProductVariant;
                 
                     // productVariant.StorageQuantity += package.Quantity;
                 
                     if (quantityUpdateProductDict.ContainsKey(productVariant))
                         quantityUpdateProductDict[productVariant] += package.Quantity;
                     else
                         quantityUpdateProductDict.Add(productVariant, package.Quantity);    
                     
                     deleteIndex.Add(stockTakeAdjustInfo.StockTakeAdjustItemsInfos[i]);
                 }
             }
             
             foreach (var stockTakeAdjustItemInfo in deleteIndex)
             {
                 stockTakeAdjustInfo.StockTakeAdjustItemsInfos.Remove(stockTakeAdjustItemInfo);
             }
             
             await _redisRepository.ReUpdateStockTakeAdjustMessage(stockTakeAdjustInfo);
             
             foreach (var keyValuePair in quantityUpdateProductDict)
             {
                 var productVariant = keyValuePair.Key;
                 productVariant.StorageQuantity = keyValuePair.Value;
                 await _productAsyncRepository.UpdateAsync(productVariant);
             }
            
             stockTakeOrder.StockTakeOrderType = StockTakeOrderType.Completed;

             stockTakeOrder.Transaction = TransactionUpdateHelper.UpdateTransaction(stockTakeOrder.Transaction,UserTransactionActionType.Modify,TransactionType.StockTake,
                 (await _userAuthentication.GetCurrentSessionUser()).Id, stockTakeOrder.Id, "");
             
             await _stAsyncRepository.UpdateAsync(stockTakeOrder);

             await _stSearchAsyncRepository.ElasticSaveSingleAsync(false,
                 IndexingHelper.StockTakeSearchIndex(stockTakeOrder), ElasticIndexConstant.STOCK_TAKE_ORDERS);
             
             var response = new STResponse();
             response.UpdatedId = stockTakeOrder.Id;
             return Ok(response);
         }
     }
      
       public class StockTakeCancel : BaseAsyncEndpoint.WithRequest<STCancelRequest>.WithResponse<STResponse>
     {
         private readonly IAsyncRepository<StockTakeOrder> _stAsyncRepository;
         private readonly IElasticAsyncRepository<StockTakeSearchIndex> _stSearchAsyncRepository;
         private readonly IAuthorizationService _authorizationService;
         private readonly IUserSession _userAuthentication;
         private readonly INotificationService _notificationService;

         public StockTakeCancel(IAsyncRepository<StockTakeOrder> stAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService, IElasticAsyncRepository<StockTakeSearchIndex> stSearchAsyncRepository1)
         {
             _stAsyncRepository = stAsyncRepository;
             _authorizationService = authorizationService;
             _userAuthentication = userAuthentication;
             _notificationService = notificationService;
             _stSearchAsyncRepository = stSearchAsyncRepository1;
         }


         [HttpPut("api/stocktake/reject")]
         [SwaggerOperation(
             Summary = "Reject a stocktake",
             Description = "Reject a stocktake",
             OperationId = "st.cancel",
             Tags = new[] { "StockTakingEndpoints" })
         ]
         public override async Task<ActionResult<STResponse>> HandleAsync(STCancelRequest request, CancellationToken cancellationToken = new CancellationToken())
         {
             if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Update))
                 return Unauthorized();
             
             var stockTakeOrder = await _stAsyncRepository.GetByIdAsync(request.StockTakeId);
             if (stockTakeOrder == null)
                 return NotFound("Can not find StockTake of id :" + request.StockTakeId);
             
             stockTakeOrder.StockTakeOrderType = StockTakeOrderType.Cancel;

             stockTakeOrder.Transaction = TransactionUpdateHelper.UpdateTransaction(stockTakeOrder.Transaction,UserTransactionActionType.Reject,TransactionType.StockTake,
                 (await _userAuthentication.GetCurrentSessionUser()).Id, stockTakeOrder.Id, request.CancelReason);
             
             await _stAsyncRepository.UpdateAsync(stockTakeOrder);
             
             await _stSearchAsyncRepository.ElasticSaveSingleAsync(false,
                 IndexingHelper.StockTakeSearchIndex(stockTakeOrder), ElasticIndexConstant.STOCK_TAKE_ORDERS);
             var currentUser = await _userAuthentication.GetCurrentSessionUser();
                  
             var messageNotification =
                 _notificationService.CreateMessage(currentUser.Fullname, "Reject","Stock Take Item", stockTakeOrder.Id);
                
             await _notificationService.SendNotificationGroup(AuthorizedRoleConstants.STOCKKEEPER,
                 currentUser.Id, messageNotification, PageConstant.STOCKTAKEORDER, stockTakeOrder.Id);

             var response = new STResponse();
             response.UpdatedId = stockTakeOrder.Id;
             return Ok(response);
         }
     }
}