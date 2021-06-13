using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Elasticsearch.Net;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetPurchaseOrderAll : BaseAsyncEndpoint.WithRequest<GetAllPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
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

        [HttpGet("api/purchaseorder/{SearchQuery}&status={Status}&page={CurrentPage}&size={SizePerPage}")]
        [SwaggerOperation(
            Summary = "Get all purchase Order",
            Description = "Get all purchase Order"  +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page \n " +
                          "{Status} Status of purchase order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync([FromRoute] GetAllPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetAllPurchaseOrderResponse();
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
                return Unauthorized();

            PagingOption<PurchaseOrderSearchIndex> pagingOption =
                new PagingOption<PurchaseOrderSearchIndex>(request.CurrentPage, request.SizePerPage);
            if (request.SearchQuery == "all")
            {
                response.IsDisplayingAll = true;
                
                var posi = await 
                    _asyncRepository.GetPOForELIndexAsync(pagingOption, request.Status, cancellationToken);
                List<PurchaseOrderSearchIndex> indexList = new List<PurchaseOrderSearchIndex>();
                
                foreach (var purchaseOrder in posi.ResultList)
                    if (purchaseOrder.Status ==
                        ((PurchaseOrderStatusType) request.Status).ToString())
                        indexList.Add(purchaseOrder);
                
                response.Paging = posi;
                response.Paging.ExecuteResourcePaging();
                return Ok(response);
            }
            else
            {
                var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>
                (
                    s => s.Index("purchaseorders").Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
                // response.PurchaseOrderSearchIndices.AddRange(responseElastic.Documents.Where(d => d.Status == ((PurchaseOrderStatusType)request.Status).ToString()));

                response.Paging = new PagingOption<PurchaseOrderSearchIndex>(request.CurrentPage,request.SizePerPage);
                response.Paging.ResultList = new List<PurchaseOrderSearchIndex>();
                
                foreach (var purchaseOrderSearchIndex in responseElastic.Documents)
                {
                    if(purchaseOrderSearchIndex.Status == ((PurchaseOrderStatusType)request.Status).ToString())
                        response.Paging.ResultList.Add(purchaseOrderSearchIndex);
                }
                response.Paging.ExecuteResourcePaging();
                return Ok(response);
            }
        }
    }
}