using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
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
            OperationId = "product.searchid",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<GetProductResponse>> HandleAsync([FromRoute] GetProductRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT_SEARCH, UserOperations.Read))
                return Unauthorized();
            
            
            var response = new GetProductResponse();
            response.IsGettingVariant = false;
            response.Product = await _asyncRepository.GetByIdAsync(request.ProductId, cancellationToken);
            response.Product.Brand.IsShowingProducts = false;
            
            return Ok(response);
        }
    }
    
    public class GetProductVariantById : BaseAsyncEndpoint.WithRequest<GetProductVariantRequest>.WithResponse<GetProductResponse>
    {
        private IAsyncRepository<ProductVariant> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetProductVariantById(IAsyncRepository<ProductVariant> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet("api/productvariant/{ProductVariantId}")]
        [SwaggerOperation(
            Summary = "Search Product by id",
            Description = "Search Product by id",
            OperationId = "product_variant.searchid",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<GetProductResponse>> HandleAsync([FromRoute] GetProductVariantRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT_SEARCH, UserOperations.Read))
                return Unauthorized();
            
            var response = new GetProductResponse();
            response.IsGettingVariant = true;
            response.ProductVariant = await _asyncRepository.GetByIdAsync(request.ProductVariantId, cancellationToken);
            
            return Ok(response);
        }
    }
}