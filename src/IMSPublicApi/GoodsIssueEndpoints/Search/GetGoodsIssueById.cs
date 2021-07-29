using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Search
{
    public class GetGoodsIssueById : BaseAsyncEndpoint.WithRequest<GiIdRequest>.WithResponse<GiSearchResponse>
    {
        private IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetGoodsIssueById(IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }
        [HttpGet("api/goodsissue/{IssueId}")]
        [SwaggerOperation(
            Summary = "Search for good issue with all or elasticsearch field",
            Description = "Search for good issue with all or elasticsearch field",
            OperationId = "gi.searchid",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        public override async Task<ActionResult<GiSearchResponse>> HandleAsync([FromRoute] GiIdRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSISSUE, UserOperations.Read))
                return Unauthorized();

            var response = new GiSearchResponse();
            response.IsForDisplay = false;
            response.GoodsIssueOrder = await _asyncRepository.GetByIdAsync(request.IssueId);
            foreach (var goodsIssueProduct in response.GoodsIssueOrder.GoodsIssueProducts)
            {
                goodsIssueProduct.IsShowingProductVariant = true;
                goodsIssueProduct.IsShowingProductVariantDetail = true;
            }
            
            return Ok(response);
        }
    }
}