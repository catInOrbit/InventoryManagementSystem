using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Elasticsearch.Net;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetPurchaseOrderAll : BaseAsyncEndpoint.WithRequest<GetPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public GetPurchaseOrderAll(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }

        [HttpGet("api/purchaseorder/{number}")]
        [SwaggerOperation(
            Summary = "Get all purchase Order",
            Description = "Get all purchase Order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync([FromRoute]GetPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetAllPurchaseOrderResponse();
            // var isAuthorized = await _authorizationService.AuthorizeAsync(
            //     HttpContext.User, "PurchaseOrder",
            //     UserOperations.Read);
            //
            // if (!isAuthorized.Succeeded)
            //     return Unauthorized();
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
                return Unauthorized();

            if (request.number == "all")
            {
                var posi = await _asyncRepository.GetPOForELIndexAsync(cancellationToken);
                response.PurchaseOrderSearchIndices = posi.ToList();
            }
            else
            {
                var pos = await _asyncRepository.ListAllAsync(cancellationToken);
                var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>(
                    s => s.Query(q => q.QueryString(d => d.Query('*' + request.number + '*'))));
                
                // var po = _asyncRepository.GetPurchaseOrderByNumber(request.number, cancellationToken);
                // response.PurchaseOrder = po;
                // return Ok(response);
                return Ok(responseElastic.Documents);
            }
            return Ok(response);
        }
    }
}