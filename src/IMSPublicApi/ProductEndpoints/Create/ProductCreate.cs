using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Create
{
    public class ProductCreate : BaseAsyncEndpoint.WithRequest<ProductCreateRequest>.WithoutResponse
    {
        private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;


        public ProductCreate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserAuthentication userAuthentication, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
        }
        
        [HttpPost("api/product/create")]
        [SwaggerOperation(
            Summary = "Create a new product",
            Description = "Create a new product",
            OperationId = "product.create",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ProductCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Create))
                return Unauthorized();
            
            var product = new ApplicationCore.Entities.Products.Product
            {
                Name = request.ProductName,
                BrandName = request.BrandName,
                CategoryId = request.CategoryId,
                CreatedDate = DateTime.Now,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                ModifiedDate = DateTime.Now,
                SellingStrategy = request.SellingStrategy,
            };

            if (request.IsVariantType)
            {
                product.IsVariantType = true;
                product.ProductVariants = request.ProductVariants;
            }
            else
            {
                product.IsVariantType = false;
                product.ProductVariants.Add(request.ProductVariants[0]);
            }

            await _asyncRepository.AddAsync(product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductSearchIndex(product));
            return Ok();
        }
    }
}