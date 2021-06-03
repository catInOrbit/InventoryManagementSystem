using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.Product
{
    public class GetProductById : BaseAsyncEndpoint.WithRequest<GetProductRequest>.WithResponse<GetProductResponse>
    {
        private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetProductById(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/product/{ProductId}")]
        [SwaggerOperation(
            Summary = "Search Product by id",
            Description = "Search Product by id",
            OperationId = "po.update",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<GetProductResponse>> HandleAsync([FromRoute] GetProductRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            var response = new GetProductResponse();
            response.Product = await _asyncRepository.GetByIdAsync(request.ProductId, cancellationToken);
            
            return Ok(response);
        }
    }
}