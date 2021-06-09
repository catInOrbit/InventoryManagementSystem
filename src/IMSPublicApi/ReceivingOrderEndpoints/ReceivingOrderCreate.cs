using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<ROCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _receiveAsyncRepository;
        private readonly IUserAuthentication _userAuthentication;

        public ReceivingOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> receiveAsyncRepository, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _receiveAsyncRepository = receiveAsyncRepository;
            _userAuthentication = userAuthentication;
        }
        
        [HttpPost("api/goodsreceipt/create")]
        [SwaggerOperation(
            Summary = "Create Goods Receipt Order",
            Description = "Create Goods Receipt Order",
            OperationId = "gr.create",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult<ROCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            
            var ro = new GoodsReceiptOrder();

            var transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.Purchase,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
            };

            ro.Transaction = transaction;
            await _receiveAsyncRepository.AddAsync(ro);
            await _receiveAsyncRepository.ElasticSaveSingleAsync(ro);

            var response = new ROCreateResponse();
            response.ReceivingOrder = ro;
            return Ok(response);
        }
    }
}