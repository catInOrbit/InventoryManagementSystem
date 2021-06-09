using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
    public class GetReceivingOrderById : BaseAsyncEndpoint.WithRequest<ROGetRequest>.WithResponse<ROGetResponse>
    {
        private readonly IAsyncRepository<GoodsReceiptOrder> _receivingOrderAsyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetReceivingOrderById(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> receivingOrderAsyncRepository)
        {
            _authorizationService = authorizationService;
            _receivingOrderAsyncRepository = receivingOrderAsyncRepository;
        }

        
        [HttpGet("api/goodsreceipt/id/{Query}")]
        [SwaggerOperation(
            Summary = "Get specific receive Order",
            Description = "Get specific receive Order",
            OperationId = "po.update",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult<ROGetResponse>> HandleAsync([FromRoute]ROGetRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
            //     return Unauthorized();
            
            var response = new ROGetResponse();
            response.IsDislayingAll = false;

            response.ReceiveingOrder = await _receivingOrderAsyncRepository.GetByIdAsync(request.Query);
            return Ok(response);
        }
    }
}