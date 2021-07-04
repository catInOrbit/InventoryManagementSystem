using System;
using System.Collections.Generic;
using System.Linq;
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
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints
{
    public class StockTakeSubmit : BaseAsyncEndpoint.WithRequest<StockTakeSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<StockTakeOrder> _asyncRepository;
        private IAsyncRepository<ProductVariant> _poAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly IUserSession _userAuthentication;

        public StockTakeSubmit(IAsyncRepository<StockTakeOrder> asyncRepository, IAuthorizationService authorizationService, IAsyncRepository<ProductVariant> poAsyncRepository, INotificationService notificationService, IUserSession userAuthentication)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _poAsyncRepository = poAsyncRepository;
            _notificationService = notificationService;
            _userAuthentication = userAuthentication;
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
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Update))
                return Unauthorized();

            var stockTakeOrder = await _asyncRepository.GetByIdAsync(request.Id);
            stockTakeOrder.StockTakeOrderType = StockTakeOrderType.Completed;
            stockTakeOrder.Transaction = TransactionUpdateHelper.UpdateTransaction(stockTakeOrder.Transaction,UserTransactionActionType.Submit, stockTakeOrder.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);
            
            
            await _asyncRepository.UpdateAsync(stockTakeOrder);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Submit","Stock Take", stockTakeOrder.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            return Ok();
        }
    }
    
     public class StockTakeUpdate : BaseAsyncEndpoint.WithRequest<STSingleUpdateRequest>.WithResponse<STUpdateResponse>
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _pvasyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;

        public StockTakeUpdate(IAsyncRepository<StockTakeOrder> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductVariant> pvasyncRepository, INotificationService notificationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _pvasyncRepository = pvasyncRepository;
            _notificationService = notificationService;
        }

        [HttpPut("api/stocktake/update")]
        [SwaggerOperation(
            Summary = "Update stock taking file",
            Description = "Update stock taking file",
            OperationId = "st.update",
            Tags = new[] { "StockTakingEndpoints" })
        ]

        public override async Task<ActionResult<STUpdateResponse>> HandleAsync(STSingleUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Update))
                return Unauthorized();

            //TODO: Fix Error
            var response = new STUpdateResponse();

            var storder = await _asyncRepository.GetByIdAsync(request.StockTakeId);
            foreach (var storderCheckItem in storder.CheckItems)
            {
                if (storderCheckItem.Id == request.StockTakeItemId)
                {
                    storderCheckItem.Note = request.Note;
                    storderCheckItem.ActualQuantity = request.ActualQuantity;

                    var productVariant = await _pvasyncRepository.GetByIdAsync(storderCheckItem.ProductVariantId);
                    
                    var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);
                    Console.WriteLine("Number of packages found: " + packages.Count);
                    int totalQuantity = 0;
                    
                    foreach (var package in packages)
                        totalQuantity += package.Quantity;

                    Console.WriteLine("Total Quantity: " + totalQuantity);

                    if (request.ActualQuantity != totalQuantity)
                        response.MismatchProductVariantId.Add(productVariant.Id);
                }
            }
            
            storder.StockTakeOrderType = StockTakeOrderType.Progressing;
            storder.Transaction = TransactionUpdateHelper.UpdateTransaction(storder.Transaction,UserTransactionActionType.Modify, storder.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);
            await _asyncRepository.UpdateAsync(storder);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                  
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update","Stock Take", storder.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok(response);
        }
    }
     
     public class StockTakeAddProduct : BaseAsyncEndpoint.WithRequest<STAddRequest>.WithResponse<STCreateItemResponse>
     {
         private readonly IAsyncRepository<StockTakeOrder> _stAsyncRepository;
         private readonly IAsyncRepository<ProductVariant> _productAsyncRepository;
         private readonly IUserSession _userAuthentication;

         private readonly INotificationService _notificationService;

         public StockTakeAddProduct(IAsyncRepository<StockTakeOrder> stAsyncRepository, IAsyncRepository<ProductVariant> productAsyncRepository, IUserSession userAuthentication, INotificationService notificationService)
         {
             _stAsyncRepository = stAsyncRepository;
             _productAsyncRepository = productAsyncRepository;
             _userAuthentication = userAuthentication;
             _notificationService = notificationService;
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
             var response = new STCreateItemResponse();
             var stockTakeOrder = await _stAsyncRepository.GetByIdAsync(request.StockTakeId);
             stockTakeOrder.CheckItems = new List<StockTakeItem>();
             foreach (var id in request.ProductIds)
             {
                 var productVariant = await _productAsyncRepository.GetByIdAsync(id);
                 
                 var stockTakeItem = new StockTakeItem
                 {
                     Note = "",
                     ProductVariantId = productVariant.Id,
                 };
                 
                 // foreach (var package in packages)
                 //     stockTakeItem.ActualQuantity += package.Quantity;
                 
                 stockTakeOrder.CheckItems.Add(stockTakeItem);
             }

             stockTakeOrder.StockTakeOrderType = StockTakeOrderType.Progressing;
             stockTakeOrder.Transaction = TransactionUpdateHelper.UpdateTransaction(stockTakeOrder.Transaction,UserTransactionActionType.Modify, stockTakeOrder.Id,
                 (await _userAuthentication.GetCurrentSessionUser()).Id);
             await _stAsyncRepository.UpdateAsync(stockTakeOrder);
             response.StockTakeOrder = stockTakeOrder;
             
             var currentUser = await _userAuthentication.GetCurrentSessionUser();
                  
             var messageNotification =
                 _notificationService.CreateMessage(currentUser.Fullname, "Add","Stock Take Item", stockTakeOrder.Id);
                
             await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                 currentUser.Id, messageNotification);
             return Ok(response);
         }
     }
     
     public class StockTakeCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<STCreateItemResponse>
     {
         private readonly IAuthorizationService _authorizationService;
         private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
         private readonly IUserSession _userAuthentication;
         private readonly INotificationService _notificationService;

         public StockTakeCreate(IAuthorizationService authorizationService, IAsyncRepository<StockTakeOrder> asyncRepository, IUserSession userAuthentication, INotificationService notificationService)
         {
             _authorizationService = authorizationService;
             _asyncRepository = asyncRepository;
             _userAuthentication = userAuthentication;
             _notificationService = notificationService;
         }

         [HttpPost("api/stocktake/create")]
         [SwaggerOperation(
             Summary = "Create a stock take order",
             Description = "Create a stock take order",
             OperationId = "st.create",
             Tags = new[] { "StockTakingEndpoints" })
         ]
         public override async Task<ActionResult<STCreateItemResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
         {
             if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Create))
                 return Unauthorized();

             var response = new STCreateItemResponse();

             var sto = new StockTakeOrder();
           
             sto.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.StockTake, sto.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);

             sto.StockTakeOrderType = StockTakeOrderType.Progressing;
             await _asyncRepository.AddAsync(sto);
             response.StockTakeOrder = sto;
             
               
             var currentUser = await _userAuthentication.GetCurrentSessionUser();
                  
             var messageNotification =
                 _notificationService.CreateMessage(currentUser.Fullname, "Create","Stock Take", sto.Id);
                
             await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                 currentUser.Id, messageNotification);
             return Ok(response);
         }
     }
}