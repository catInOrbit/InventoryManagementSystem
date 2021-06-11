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

        [HttpGet("api/goodsissue/search/{SearchQuery}&currentPage={CurrentPage}&sizePerPage={SizePerPage}")]
        [SwaggerOperation(
            Summary = "Search for good issue with all or elasticsearch field",
            Description = "Search for good issue with all or elasticsearch field",
            OperationId = "gi.search",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        public override async Task<ActionResult<GiSearchResponse>> HandleAsync([FromRoute]GiSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "GoodsIssue", UserOperations.Read))
                return Unauthorized();

            var response = new GiSearchResponse();
            PagingOption<GoodsIssueOrder> pagingOption = new PagingOption<GoodsIssueOrder>(
                request.CurrentPage, request.SizePerPage);
            
            if (request.SearchQuery == "all")
            {
                response.IsForDisplay = true;
                var gis = await _asyncRepository.ListAllAsync(pagingOption, cancellationToken);
                foreach (var goodsIssueOrder in gis.ResultList)
                {
                    if (goodsIssueOrder.GoodsIssueType == GoodsIssueType.Pending)
                    {
                        var giDisplay = new GIDisplay()
                        {
                            Id = goodsIssueOrder.Id,
                            DeliveryDate = goodsIssueOrder.DeliveryDate,
                            CreatedByName = goodsIssueOrder.Transaction.CreatedBy.Fullname,
                            GoodsIssueNumber = goodsIssueOrder.GoodsIssueNumber
                        };
                        
                        response.GoodsIssueOrdersDisplays.Add(giDisplay);
                    }
                }
                    
                return Ok(response);
            }

            else
            {
                var responseElastic = await _elasticClient.SearchAsync<GoodsIssueSearchIndex>(
                    s => s.From(request.CurrentPage).Size(request.SizePerPage).Index("goodsissueorders").Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
                return Ok(responseElastic.Documents);
            }
        }
    }
}