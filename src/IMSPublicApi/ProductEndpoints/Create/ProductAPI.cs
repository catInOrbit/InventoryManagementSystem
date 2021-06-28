using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
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
    public class ProductCreate : BaseAsyncEndpoint.WithRequest<ProductRequest>.WithResponse<ProductCreateResponse>
    {
        private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;

        private readonly INotificationService _notificationService;

        public ProductCreate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos, INotificationService notificationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
        }
        
        [HttpPost("api/product/create")]
        [SwaggerOperation(
            Summary = "Create a new product",
            Description = "Create a new product",
            OperationId = "product.create",
            Tags = new[] { "ProductEndpoints" })
        ]

        public override async Task<ActionResult<ProductCreateResponse>> HandleAsync(ProductRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
               if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Create))
                return Unauthorized();

            ApplicationCore.Entities.Products.Product product = new ApplicationCore.Entities.Products.Product
            {
                Name = request.Name,
                BrandName = request.BrandName,
                CategoryId = request.CategoryId,
            };

            product.Transaction = new Transaction
            {
                Name = "Created Product " + product.Id,
                CreatedDate = DateTime.Now,
                Type = TransactionType.NewProduct,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                TransactionStatus = true
            };

            product.TransactionId = product.Transaction.Id;
            product.IsVariantType = request.IsVariantType;
            product.ProductVariants = new List<ProductVariant>();
            foreach (var productVairantRequestInfo in request.ProductVariants)
            {
                var productVariant = new ProductVariant
                {
                    Name = productVairantRequestInfo.Name,
                    Sku = productVairantRequestInfo.Sku,
                    Unit = productVairantRequestInfo.Unit,
                    StorageQuantity = productVairantRequestInfo.StorageQuantity,
                    IsVariantType = product.IsVariantType,
                    Barcode = productVairantRequestInfo.Barcode,
                    Transaction = new Transaction
                    {
                        CreatedDate = DateTime.Now,
                        Type = TransactionType.NewProduct,
                        CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                        TransactionStatus = true
                    }
                };

                productVariant.Transaction.Name = "Created Product Variant" + productVariant.Id;

                product.ProductVariants.Add(productVariant);
            }

            if (!product.IsVariantType)
                product.Name = product.ProductVariants.ToList()[0].Name;
            
            await _asyncRepository.AddAsync(product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductSearchIndex(product),ElasticIndexConstant.PRODUCT_INDICES);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();

            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create", "Product", product.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            var response = new ProductCreateResponse
            {
                CreatedProductId = product.Id
            };
            return Ok(response);
        }
    }
    
}