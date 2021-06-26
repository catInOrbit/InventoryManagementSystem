using System;
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
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote.Create
{
    public class PriceQuoteRequestCreate : BaseAsyncEndpoint.WithRequest<PQCreateRequest>.WithResponse<PQCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<Product> _productRepos;
        
        private INotificationService _notificationService;


        public PriceQuoteRequestCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserSession userAuthentication,  IAsyncRepository<Product> productRepos, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, INotificationService notificationService)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productRepos = productRepos;
            _indexAsyncRepository = indexAsyncRepository;
            _notificationService = notificationService;
        }
        
        [HttpPost("api/pricequote/create/{Id}")]
        [SwaggerOperation(
            Summary = "Create price quote request with Id from purchase requisition",
            Description = "Create price quote request with Id from purchase requisition",
            OperationId = "po.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]

        public override async Task<ActionResult<PQCreateResponse>> HandleAsync([FromRoute]PQCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRICEQUOTEORDER, UserOperations.Create))
                return Unauthorized();
            
            var response = new PQCreateResponse();
            var po = await _asyncRepository.GetByIdAsync(request.Id);
            var transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.PriceQuote,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                TransactionStatus = true
            };

            po.PurchaseOrderStatus = PurchaseOrderStatusType.PQCreated;
            po.Transaction = transaction;
            response.PurchaseOrderPQ = po;
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false,IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);
            
                  
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create", "Price Quote", po.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok(response);
        }
    }
}