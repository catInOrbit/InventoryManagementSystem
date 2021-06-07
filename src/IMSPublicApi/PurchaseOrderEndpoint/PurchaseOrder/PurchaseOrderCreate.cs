using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
     
    public class PurchaseOrderCreate : BaseAsyncEndpoint.WithRequest<POCreateRequest>.WithResponse<POCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _purchaseOrderRepos;
        private readonly IAsyncRepository<PriceQuoteOrder> _priceQuoteRepos;

        private readonly IUserAuthentication _userAuthentication;

        public PurchaseOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> purchaseOrderRepos, IUserAuthentication userAuthentication, IAsyncRepository<PriceQuoteOrder> priceQuoteRepos)
        {
            _authorizationService = authorizationService;
            _purchaseOrderRepos = purchaseOrderRepos;
            _userAuthentication = userAuthentication;
            _priceQuoteRepos = priceQuoteRepos;
        }

        [HttpPost("api/purchaseorder/create")]
        [SwaggerOperation(
            Summary = "Create purchase order",
            Description = "Create purchase order",
            OperationId = "po.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<POCreateResponse>> HandleAsync(POCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new POCreateResponse();

            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Create))
                return Unauthorized();

            var purchaseOrder = new ApplicationCore.Entities.Orders.PurchaseOrder();

            var transaction = new Transaction
            {
                TransactionId = purchaseOrder .Id,
                TransactionNumber = DateTime.UtcNow.Date.ToString("ddMMyyyy") + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                CreatedDate = DateTime.Now,
                Type = TransactionType.Purchase,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
            };

            purchaseOrder.Transaction = transaction;
            
            var pqData = _priceQuoteRepos.GetPriceQuoteByNumber(request.PriceQuoteNumber);
            purchaseOrder.PurchaseOrderStatus = PurchaseOrderStatusType.Created;
            if (pqData != null)
            {
                purchaseOrder.Transaction.TransactionNumber = pqData.Transaction.TransactionNumber;
                purchaseOrder.PurchaseOrderProduct = pqData.PurchaseOrderProduct;
                purchaseOrder.SupplierId = pqData.SupplierId;
            }

            response.PurchaseOrder = purchaseOrder;
            await _purchaseOrderRepos.AddAsync(purchaseOrder);
            await _purchaseOrderRepos.ElasticSaveSingleAsync(purchaseOrder);
            return Ok(response);
        }
    }
}