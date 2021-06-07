using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
    public class GetReceivingOrder : BaseAsyncEndpoint.WithRequest<ROGetRequest>.WithResponse<ROGetResponse>
    {
        private IAuthorizationService _authorizationService;
        private IAsyncRepository<GoodsReceiptOrder> _asyncRepository;
        private readonly IElasticClient _elasticClient;

        public GetReceivingOrder(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> asyncRepository, IElasticClient elasticClient)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _elasticClient = elasticClient;
        }
        
        
        [HttpGet("api/receive/{Query}")]
        [SwaggerOperation(
            Summary = "Get all receive Order",
            Description = "Get all Receive Order, api/receive/{Query} to get all ",
            OperationId = "po.update",
            Tags = new[] { "ReceiveOrderEndpoints" })
        ]
        public override async Task<ActionResult<ROGetResponse>> HandleAsync([FromRoute] ROGetRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            //
            // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
            //     return Unauthorized();

            var response = new ROGetResponse();
            response.IsDislayingAll = true;
            if (request.Query == "all")
            {
                var posi = await _asyncRepository.GetROForELIndexAsync(cancellationToken);
                response.ReceiveingOrderSearchIndex = posi.ToList();
            }
            else
            {
                var pos = await _asyncRepository.ListAllAsync(cancellationToken);
                var responseElastic = await _elasticClient.SearchAsync<ReceivingOrderSearchIndex>(
                    s => s.Index("receivingorders").Query(q =>q.QueryString(d =>d.Query('*' + request.Query + '*'))));
                return Ok(responseElastic.Documents);
            }

            return Ok(response);
        }
    }
}