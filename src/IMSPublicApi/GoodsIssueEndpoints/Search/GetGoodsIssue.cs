using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
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

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class GetGoodsIssue : BaseAsyncEndpoint.WithRequest<GiSearchRequest>.WithResponse<GiSearchResponse>
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
            Summary = "Search for good issue with all or elasticsearch field",
            Description = "Search for good issue with all or elasticsearch field" +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page",
            OperationId = "gi.search",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        [HttpGet("api/goodsissue/search/{SearchQuery}&currentPage={CurrentPage}&sizePerPage={SizePerPage}")]
        public override async Task<ActionResult<GiSearchResponse>> HandleAsync([FromRoute]GiSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "GoodsIssue", UserOperations.Read))
                return Unauthorized();

            var response = new GiSearchResponse();
            PagingOption<GoodsIssueSearchIndex> pagingOption = new PagingOption<GoodsIssueSearchIndex>(
                request.CurrentPage, request.SizePerPage);
            response.IsForDisplay = true;

            if (request.SearchQuery == "all")
            {
                
                response.Paging = await _asyncRepository.GetGIForELIndexAsync(pagingOption, cancellationToken);
              
                    
                return Ok(response);
            }

            else
            {
                var responseElastic = await _elasticClient.SearchAsync<GoodsIssueSearchIndex>(
                    s => s.Index("goodsissueorders").Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
                
                foreach (var goodsIssueSearchIndex in responseElastic.Documents)
                {
                    pagingOption.ResultList.Add(goodsIssueSearchIndex);
                }
                pagingOption.ExecuteResourcePaging();
                response.Paging = pagingOption;
                return Ok(response);
            }
        }
    }
}