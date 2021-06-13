using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
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
        
        
        [HttpGet("api/goodsreceipt/{Query}&page={CurrentPage}&size={SizePerPage}")]
        [SwaggerOperation(
            Summary = "Get all receive Order",
            Description = "Get all Receive Order"+
                          "\n {Query}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page" ,
            OperationId = "ro.search",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult<ROGetResponse>> HandleAsync([FromRoute] ROGetRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
                return Unauthorized();
            
            PagingOption<GoodsReceiptOrderSearchIndex> pagingOption = new PagingOption<GoodsReceiptOrderSearchIndex>(
                request.CurrentPage, request.SizePerPage);

            var response = new ROGetResponse();
            response.IsDislayingAll = true;
            if (request.Query == "all")
                response.Paging  = await _asyncRepository.GetROForELIndexAsync(pagingOption, cancellationToken);
            
            
            else
            {
                var responseElastic = await _elasticClient.SearchAsync<GoodsReceiptOrderSearchIndex>(
                    s => s.Index("receivingorders").Query(q =>q.QueryString(d =>d.Query('*' + request.Query + '*'))));
                
                foreach (var goodsReceiptOrderSearchIndex in responseElastic.Documents)
                    pagingOption.ResultList.Add(goodsReceiptOrderSearchIndex);
                pagingOption.ExecuteResourcePaging();

                response.Paging = pagingOption;
            }

            return Ok(response);
        }
    }
}