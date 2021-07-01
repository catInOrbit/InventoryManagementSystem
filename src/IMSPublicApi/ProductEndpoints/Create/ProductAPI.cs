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
        private readonly IAsyncRepository<ProductVariantSearchIndex> _productIndexAsyncRepositoryRepos;

        private readonly INotificationService _notificationService;

        public ProductCreate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductVariantSearchIndex> productIndexAsyncRepositoryRepos, INotificationService notificationService)
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

           
            product.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.Product,
                product.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);


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
                    IsVariantType = product.IsVariantType,
                    Barcode = productVairantRequestInfo.Barcode,
                    Price = productVairantRequestInfo.Price,
                    Cost = productVairantRequestInfo.SalePrice,
                    
                };
                productVariant.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.ProductVariant, productVariant.Id, 
                    (await _userAuthentication.GetCurrentSessionUser()).Id);

                var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);

                foreach (var package in packages)
                {
                    productVariant.StorageQuantity += package.Quantity;
                }

                productVariant.Transaction.Name = "Created Product Variant" + productVariant.Id;

                product.ProductVariants.Add(productVariant);
            }

            // if (!product.IsVariantType)
            //     product.Name = product.ProductVariants.ToList()[0].Name;
            
            await _asyncRepository.AddAsync(product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductVariantSearchIndex(product),ElasticIndexConstant.PRODUCT_INDICES);
            
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
        public class ProductUpdate : BaseAsyncEndpoint.WithRequest<ProductUpdateRequest>.WithoutResponse
    {
        private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductVariantSearchIndex> _productIndexAsyncRepositoryRepos;

        private readonly INotificationService _notificationService;

        public ProductUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductVariantSearchIndex> productIndexAsyncRepositoryRepos, INotificationService notificationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
        }
        
        [HttpPut("api/product/update")]
        [SwaggerOperation(
            Summary = "Update a new product",
            Description = "Update a new product",
            OperationId = "product.create",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ProductUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
               if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Update))
                return Unauthorized();

               var product = await _asyncRepository.GetByIdAsync(request.Id);


           product.Name = request.Name;
           product.BrandName = request.BrandName;
           product.CategoryId = request.CategoryId;
            
           product.Transaction = TransactionUpdateHelper.UpdateTransaction(product.Transaction,UserTransactionActionType.Modify, product.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);

            product.TransactionId = product.Transaction.Id;
            product.IsVariantType = request.IsVariantType;
            var listNewVariant = new List<ProductVairantUpdateRequestInfo>(request.ProductVariantsUpdate);
            if (product.IsVariantType)
            {
                foreach (var productVairantRequestInfo in request.ProductVariantsUpdate)
                {
                    foreach (var productVariantSystemList in product.ProductVariants)
                    {
                        if (productVariantSystemList.Id != null && productVariantSystemList.Id == productVairantRequestInfo.Id)
                        {
                            productVariantSystemList.Name = productVairantRequestInfo.Name;
                            productVariantSystemList.Sku = productVairantRequestInfo.Sku;
                            productVariantSystemList.Unit = productVairantRequestInfo.Unit;
                            productVariantSystemList.IsVariantType = product.IsVariantType;
                            productVariantSystemList.Barcode = productVairantRequestInfo.Barcode;
                        }

                        else if(productVariantSystemList.Id == null)
                            listNewVariant.Remove(productVairantRequestInfo);
                    }
                }

                foreach (var productVairantRequestInfo in listNewVariant)
                {
                    var productVariant = new ProductVariant
                    {
                        Name = productVairantRequestInfo.Name,
                        Sku = productVairantRequestInfo.Sku,
                        Unit = productVairantRequestInfo.Unit,
                        IsVariantType = product.IsVariantType,
                        Barcode = productVairantRequestInfo.Barcode,
                        Price = productVairantRequestInfo.Price,
                        Cost = productVairantRequestInfo.SalePrice,
                        ProductId = product.Id,
                    };
                    
                    productVariant.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.ProductVariant, productVariant.Id, 
                        (await _userAuthentication.GetCurrentSessionUser()).Id);
                        
                    var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);

                    foreach (var package in packages)
                        productVariant.StorageQuantity += package.Quantity;

                    productVariant.Transaction.Name = "Created Product Variant" + productVariant.Id;
                    product.ProductVariants.Add(productVariant);
                }
            }

            else
            {
                product.ProductVariants.Clear();
                var productVairantUpdateRequestInfo = request.ProductVariantsUpdate[0];
                var productVariant = new ProductVariant
                {
                    Name = productVairantUpdateRequestInfo.Name,
                    Sku = productVairantUpdateRequestInfo.Sku,
                    Unit = productVairantUpdateRequestInfo.Unit,
                    IsVariantType = product.IsVariantType,
                    Barcode = productVairantUpdateRequestInfo.Barcode,
                    Price = productVairantUpdateRequestInfo.Price,
                    Cost = productVairantUpdateRequestInfo.SalePrice,
                    ProductId = product.Id
                };
                
                productVariant.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.ProductVariant, productVariant.Id, 
                    (await _userAuthentication.GetCurrentSessionUser()).Id);

                
                var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);

                foreach (var package in packages)
                    productVariant.StorageQuantity += package.Quantity; 
                
                productVariant.Transaction.Name = "Created Product Variant" + productVariant.Id;
                product.ProductVariants.Add(productVariant);
            }
            
            await _asyncRepository.UpdateAsync(product);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
            // await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false, IndexingHelper.ProductSearchIndex(product),ElasticIndexConstant.PRODUCT_INDICES);
            
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update", "Product", product.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            return Ok();
        }
    }

}