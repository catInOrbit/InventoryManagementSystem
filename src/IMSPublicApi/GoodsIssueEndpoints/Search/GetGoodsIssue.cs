using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class SearchGoodsIssue : BaseAsyncEndpoint.WithRequest<GISearchRequest>.WithResponse<GiSearchResponse>
    {
        private IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public SearchGoodsIssue(IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }

        [SwaggerOperation(
            Summary = "Get all good issue with status (requisition at 0)",
            Description = "Get all good issue with status (requisition at 0)" +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page",
            OperationId = "gi.searchall",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        [HttpGet("api/goodsissue/search")]
        public override async Task<ActionResult<GiSearchResponse>> HandleAsync([FromQuery]GISearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSISSUE, UserOperations.Read))
                return Unauthorized();

            var response = new GiSearchResponse();
            PagingOption<GoodsIssueSearchIndex> pagingOption = new PagingOption<GoodsIssueSearchIndex>(
                request.CurrentPage, request.SizePerPage);
            response.IsForDisplay = true;
            
            
            var searchFilter = new GISearchFilter
            {
                FromStatus = request.FromStatus,
                ToStatus = request.ToStatus,
                CreatedByName = request.CreatedByName,
                FromCreatedDate = request.FromCreatedDate,
                FromDeliveryDate = request.FromDeliveryDate,
                ToCreatedDate = request.ToCreatedDate,
                ToDeliveryDate = request.ToDeliveryDate,
                DeliveryMethod = request.DeliveryMethod
            };

            
            ISearchResponse<GoodsIssueSearchIndex> responseElastic;
            
            ElasticSearchHelper<GoodsIssueSearchIndex> elasticSearchHelper = new ElasticSearchHelper<GoodsIssueSearchIndex>(_elasticClient, request.SearchQuery,
                ElasticIndexConstant.GOODS_ISSUE_ORDERS);
            responseElastic = await elasticSearchHelper.GetDocuments();

            pagingOption.ResultList = FilteringService.GoodsIssueIndexFiltering(responseElastic.Documents.ToList(), searchFilter,
                new CancellationToken());
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
}