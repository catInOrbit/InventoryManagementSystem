using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
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

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class GetGoodsIssue : BaseAsyncEndpoint.WithRequest<GIAllRequest>.WithResponse<GiSearchResponse>
    {
        private IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public GetGoodsIssue(IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
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
            OperationId = "gi.search",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        [HttpPost("api/goodsissue/all")]
        public override async Task<ActionResult<GiSearchResponse>> HandleAsync(GIAllRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "GoodsIssue", UserOperations.Read))
                return Unauthorized();

            var response = new GiSearchResponse();
            PagingOption<GoodsIssueSearchIndex> pagingOption = new PagingOption<GoodsIssueSearchIndex>(
                request.CurrentPage, request.SizePerPage);
            response.IsForDisplay = true;

            GISearchFilter searchFilter = new GISearchFilter();
            searchFilter.Status = request.GiSearchFilter.Status;
            var gisi = await _asyncRepository.GetGIForELIndexAsync(pagingOption, searchFilter, cancellationToken);
            response.Paging = pagingOption;
            
            response.Paging = gisi;
            return Ok(response);
        }
    }
    
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
            OperationId = "gi.search",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        [HttpPost("api/goodsissue/search")]
        public override async Task<ActionResult<GiSearchResponse>> HandleAsync(GISearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "GoodsIssue", UserOperations.Read))
                return Unauthorized();

            var response = new GiSearchResponse();
            PagingOption<GoodsIssueSearchIndex> pagingOption = new PagingOption<GoodsIssueSearchIndex>(
                request.CurrentPage, request.SizePerPage);
            response.IsForDisplay = true;

            if (request.SearchQuery == "")
            {
                response.Paging  = await _asyncRepository.GetGIForELIndexAsync(pagingOption, request.SearchFilter, cancellationToken);
                return Ok(response);
            }
            
            var responseElastic = await _elasticClient.SearchAsync<GoodsIssueSearchIndex>(
                s => s.Size(2000).Index(ElasticIndexConstant.GOODS_ISSUE_ORDERS).Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
            
            pagingOption.ResultList = _asyncRepository.GoodsIssueIndexFiltering(responseElastic.Documents.ToList(), request.SearchFilter,
                new CancellationToken());
            
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
}