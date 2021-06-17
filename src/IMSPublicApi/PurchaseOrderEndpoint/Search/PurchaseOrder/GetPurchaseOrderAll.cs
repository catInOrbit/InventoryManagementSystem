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

        [HttpGet("api/purchaseorder/all")]
        [SwaggerOperation(
            Summary = "Get all purchase Order",
            Description = "Get all purchase Order"  +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page \n " +
                          "{Status} Status of purchase order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync(GetAllPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetAllPurchaseOrderResponse();
            // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
            //     return Unauthorized();

            PagingOption<PurchaseOrderSearchIndex> pagingOption =
                 new PagingOption<PurchaseOrderSearchIndex>(request.CurrentPage, request.SizePerPage);
            response.IsDisplayingAll = true;
            
            var posi = await 
                _asyncRepository.GetPOForELIndexAsync(pagingOption, new POSearchFilter(){Status = request.PoSearchFilter.Status}, cancellationToken);
            
            response.Paging = posi;
            return Ok(response);
        }
    }
    
      public class SearchPurchaseOrder : BaseAsyncEndpoint.WithRequest<SearchPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public SearchPurchaseOrder(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }

        [HttpGet("api/purchaseorder/search/{SearchQuery}&status={Status}&page={CurrentPage}&size={SizePerPage}")]
        [SwaggerOperation(
            Summary = "Search purchase Order",
            Description = "Search purchase Order"  +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page \n " +
                          "{Status} Status of purchase order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync([FromRoute] SearchPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetAllPurchaseOrderResponse();
            // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
            //     return Unauthorized();

            PagingOption<PurchaseOrderSearchIndex> pagingOption =
                new PagingOption<PurchaseOrderSearchIndex>(request.CurrentPage, request.SizePerPage);
            var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>
            (
                s => s.Index("purchaseorders").Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));

            foreach (var purchaseOrderSearchIndex in responseElastic.Documents)
            {
                if (request.Status != -99)
                {
                    if(purchaseOrderSearchIndex.Status == ((PurchaseOrderStatusType)request.Status).ToString())
                        pagingOption.ResultList.Add(purchaseOrderSearchIndex);    
                }
                else
                    pagingOption.ResultList.Add(purchaseOrderSearchIndex);    
            }
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
    
    
}