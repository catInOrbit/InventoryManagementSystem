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
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Create))
                return Unauthorized();
            
            await _asyncRepository.AddAsync(request.Product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductSearchIndex(request.Product));
            return Ok();
        }
    }
    
     public class ProductUpdate : BaseAsyncEndpoint.WithRequest<ProductUpdateRequest>.WithoutResponse
    {
        private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;


        public ProductUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserAuthentication userAuthentication, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
        }
        
        [HttpPost("api/product/update")]
        [SwaggerOperation(
            Summary = "Update info of a product",
            Description = "Update info of a product",
            OperationId = "product.update",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ProductUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Create))
                return Unauthorized();
            
            var product = await _asyncRepository.GetByIdAsync(request.Id);
            product = request.ProductUpdate;
            await _asyncRepository.UpdateAsync(product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductSearchIndex(product));
            return Ok();
        }
    }
}