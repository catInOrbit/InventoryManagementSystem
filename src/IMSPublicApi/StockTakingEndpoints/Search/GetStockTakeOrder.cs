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
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Search
{
    public class GetStockTakeOrder : BaseAsyncEndpoint.WithRequest<STSearchRequest>.WithResponse<STSearchResponse>
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public GetStockTakeOrder(IAuthorizationService authorizationService, IElasticClient elasticClient, IAsyncRepository<StockTakeOrder> asyncRepository)
        {
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
            _asyncRepository = asyncRepository;
        }

        [HttpGet("api/stocktake/search")]
        [SwaggerOperation(
            Summary = "Get all stock take Order or specific with search query",
            Description = "Get all stock take Order or specific with search query"+
                          "\n {SearchQuery}: Query to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page",
            OperationId = "st.search",
            Tags = new[] { "StockTakingEndpoints" })
        ]

        public override async Task<ActionResult<STSearchResponse>> HandleAsync([FromQuery] STSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.STOCKTAKEORDER, UserOperations.Read))
                return Unauthorized();
            
            PagingOption<StockTakeSearchIndex> pagingOption = new PagingOption<StockTakeSearchIndex>(
                request.CurrentPage, request.SizePerPage);

            var response = new STSearchResponse();
            response.IsDisplayingAll = true;
            
            var stSearchFilter = new STSearchFilter
            {
                FromStatus = request.FromStatus,
                ToStatus = request.ToStatus,
                CreatedByName = request.CreatedByName,
                FromCreatedDate = request.FromCreatedDate,
                FromDeliveryDate = request.FromDeliveryDate,
                ToCreatedDate = request.ToCreatedDate,
                ToDeliveryDate = request.ToDeliveryDate,
            };
            
               
            ISearchResponse<StockTakeSearchIndex> responseElastic;
      
            ElasticSearchHelper<StockTakeSearchIndex> elasticSearchHelper = new ElasticSearchHelper<StockTakeSearchIndex>(_elasticClient, request.SearchQuery,
                ElasticIndexConstant.STOCK_TAKE_ORDERS);
            responseElastic = await elasticSearchHelper.GetDocuments();
            
            pagingOption.ResultList = _asyncRepository.StockTakeIndexFiltering(responseElastic.Documents.ToList(), stSearchFilter,
                new CancellationToken());
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
}