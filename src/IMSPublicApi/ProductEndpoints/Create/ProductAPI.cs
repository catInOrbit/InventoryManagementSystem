using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Create
{
    public class ProductCreate : BaseAsyncEndpoint.WithRequest<ProductRequest>.WithoutResponse
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
        public override async Task<ActionResult> HandleAsync(ProductRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Create))
            //     return Unauthorized();

            ApplicationCore.Entities.Products.Product product = new ApplicationCore.Entities.Products.Product
            {
                Name = request.Name,
                BrandName = request.BrandName,
                CategoryId = request.CategoryId,
                SellingStrategy = request.SellingStrategy,
            };

            product.Transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.NewProduct,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
            };

            if (request.ProductVariants.Count > 0)
            {
                product.IsVariantType = true;
                product.ProductVariants = new List<ProductVariant>();
                foreach (var productVairantRequestInfo in request.ProductVariants)
                {
                    var productVariant = new ProductVariant
                    {
                        Name = productVairantRequestInfo.Name,
                        Sku = productVairantRequestInfo.Sku,
                        Unit = productVairantRequestInfo.Unit,
                        StorageLocation = productVairantRequestInfo.StorageLocation,
                        StorageQuantity = productVairantRequestInfo.StorageQuantity,
                        IsVariantType = product.IsVariantType,
                        Transaction = new Transaction
                        {
                            CreatedDate = DateTime.Now,
                            Type = TransactionType.NewProduct,
                            CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
                        }
                    };
                    
                    productVariant.VariantValues = new List<VariantValue>();
                    foreach (var variantValueRequestInfo in productVairantRequestInfo.VariantValues)
                    {
                        var variantValue = new VariantValue
                        {
                            Attribute = variantValueRequestInfo.Attribute,
                            Value = variantValueRequestInfo.Value,
                            ProductVariantId = productVariant.Id,
                        };

                        productVariant.VariantValues.Add(variantValue);
                    }
                    product.ProductVariants.Add(productVariant);
                }
            }
            
            await _asyncRepository.AddAsync(product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductSearchIndex(product));
            return Ok();
        }
    }
    
    //  public class ProductUpdate : BaseAsyncEndpoint.WithRequest<ProductRequest>.WithoutResponse
    // {
    //     private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
    //     private readonly IAuthorizationService _authorizationService;
    //     private readonly IUserAuthentication _userAuthentication;
    //     private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;
    //
    //
    //     public ProductUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserAuthentication userAuthentication, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos)
    //     {
    //         _asyncRepository = asyncRepository;
    //         _authorizationService = authorizationService;
    //         _userAuthentication = userAuthentication;
    //         _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
    //     }
    //     
    //     [HttpPost("api/product/update")]
    //     [SwaggerOperation(
    //         Summary = "Update info of a product",
    //         Description = "Update info of a product",
    //         OperationId = "product.update",
    //         Tags = new[] { "ProductEndpoints" })
    //     ]
    //     public override async Task<ActionResult> HandleAsync(ProductRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Create))
    //         //     return Unauthorized();
    //         
    //         var product = await _asyncRepository.GetByIdAsync(request.Id);
    //         product = request.ProductUpdate;
    //         await _asyncRepository.UpdateAsync(product);
    //         await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductSearchIndex(product));
    //         return Ok();
    //     }
    // }
}