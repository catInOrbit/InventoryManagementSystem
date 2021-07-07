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
                Brand = new Brand
                {
                    BrandDescription = request.BrandDescription,
                    BrandName = request.BrandName
                },
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
        public class ProductVariantUpdate : BaseAsyncEndpoint.WithRequest<ProductVariantUpdateRequest>.WithResponse<ProductUpdateResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;

        private readonly IAsyncRepository<Package> _pacakgeAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductVariantSearchIndex> _productVariantIndexAsyncRepositoryRepos;
        private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;

        private readonly IRedisRepository _redisRepository;

        private readonly INotificationService _notificationService;

        public ProductVariantUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductVariantSearchIndex> productVariantIndexAsyncRepositoryRepos, INotificationService notificationService, IRedisRepository redisRepository, IAsyncRepository<Package> pacakgeAsyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productVariantIndexAsyncRepositoryRepos = productVariantIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
            _redisRepository = redisRepository;
            _pacakgeAsyncRepository = pacakgeAsyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
        }
        
        [HttpPut("api/productvariant/update")]
        [SwaggerOperation(
            Summary = "Update a new product",
            Description = "Update a new product",
            OperationId = "product.create",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<ProductUpdateResponse>> HandleAsync(ProductVariantUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
           if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Update))
            return Unauthorized();

           var product = await _asyncRepository.GetByIdAsync(request.ProductId);
               
           product.Transaction = TransactionUpdateHelper.UpdateTransaction(product.Transaction,UserTransactionActionType.Modify,
                (await _userAuthentication.GetCurrentSessionUser()).Id, product.Id, "");

            product.TransactionId = product.Transaction.Id;
            product.IsVariantType = request.IsVariantType;
            var listNewVariant = new List<ProductVairantUpdateRequestInfo>(request.ProductVariantsUpdate);
            

            if (product.IsVariantType)
            {
                foreach (var productVairantRequestInfo in request.ProductVariantsUpdate)
                {
                    foreach (var productVariantSystemList in product.ProductVariants)
                    {
                        if (productVairantRequestInfo.Id != null && productVariantSystemList.Id == productVairantRequestInfo.Id)
                        {
                            productVariantSystemList.Name = productVairantRequestInfo.Name;
                            productVariantSystemList.Sku = productVairantRequestInfo.Sku;
                            productVariantSystemList.Unit = productVairantRequestInfo.Unit;
                            productVariantSystemList.IsVariantType = product.IsVariantType;
                            productVariantSystemList.Barcode = productVairantRequestInfo.Barcode;
                            productVariantSystemList.Price = productVairantRequestInfo.Price;

                            await _productVariantIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false, IndexingHelper.ProductVariantSearchIndex(productVariantSystemList),ElasticIndexConstant.PRODUCT_VARIANT_INDICES);

                            listNewVariant.Remove(productVairantRequestInfo);
                        }
                    }
                    await _redisRepository.RemoveProductUpdateMessage("ProductUpdateMessage", productVairantRequestInfo.Id);
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
                        ProductId = product.Id,
                    };
                    
                    productVariant.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.ProductVariant, productVariant.Id, 
                        (await _userAuthentication.GetCurrentSessionUser()).Id);
                        
                    var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);

                    foreach (var package in packages)
                    {
                        productVariant.StorageQuantity += package.Quantity;
                        await _pacakgeAsyncRepository.UpdateAsync(package);
                    }

                    productVariant.Packages = packages;
                    await _productVariantAsyncRepository.AddAsync(productVariant);
                    await _productVariantIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true, IndexingHelper.ProductVariantSearchIndex(productVariant),ElasticIndexConstant.PRODUCT_VARIANT_INDICES);

                    productVariant.Transaction.TransactionRecord[^1].Name = "Created Product Variant" + productVariant.Id;
                    product.ProductVariants.Add(productVariant);
                }
            }

            else
            {
                foreach (var productProductVariant in product.ProductVariants)
                {
                    productProductVariant.Transaction.TransactionStatus = false;
                    productProductVariant.Transaction.Type = TransactionType.Deleted;
                    await _productVariantAsyncRepository.UpdateAsync(productProductVariant);
                    // await _productVariantIndexAsyncRepositoryRepos.ElasticDeleteSingleAsync(IndexingHelper.ProductVariantSearchIndex(productProductVariant),ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
                }
                
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
                    ProductId = product.Id,
                    Product = product
                };

                
                productVariant.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.ProductVariant, productVariant.Id, 
                    (await _userAuthentication.GetCurrentSessionUser()).Id);

                
                var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);

                foreach (var package in packages)
                {
                    productVariant.StorageQuantity += package.Quantity; 
                    await _pacakgeAsyncRepository.UpdateAsync(package);
                }

                productVariant.Packages = packages;
                await _productVariantAsyncRepository.AddAsync(productVariant);
                product.ProductVariants.Add(productVariant);
                
                await _productVariantIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true, IndexingHelper.ProductVariantSearchIndex(productVariant),ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
                await _redisRepository.RemoveProductUpdateMessage("ProductUpdateMessage", productVariant.Id);
            }
            
            await _asyncRepository.UpdateAsync(product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false, IndexingHelper.ProductSearchIndex(product),ElasticIndexConstant.PRODUCT_INDICES);

            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
            
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update", "Product", product.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            var response = new ProductUpdateResponse();
            response.Product = product;
            return Ok(response);
        }
    }
           public class ProductUpdate : BaseAsyncEndpoint.WithRequest<ProductUpdateRequest>.WithResponse<ProductUpdateResponse>
    {
        private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;
        private readonly IRedisRepository _redisRepository;

        private readonly INotificationService _notificationService;

        public ProductUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos, INotificationService notificationService, IRedisRepository redisRepository)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
            _redisRepository = redisRepository;
        }
        
        [HttpPut("api/product/update")]
        [SwaggerOperation(
            Summary = "Update a new product",
            Description = "Update a new product",
            OperationId = "product.create",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<ProductUpdateResponse>> HandleAsync(ProductUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
           if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Update))
            return Unauthorized();

           var product = await _asyncRepository.GetByIdAsync(request.Id);
               
           product.Name = request.Name;
           product.Brand.BrandName = request.BrandName;
           product.Brand.BrandDescription = request.BrandDescription;
           product.CategoryId = request.CategoryId;
            
           product.Transaction = TransactionUpdateHelper.UpdateTransaction(product.Transaction,UserTransactionActionType.Modify,
                (await _userAuthentication.GetCurrentSessionUser()).Id, product.Id, "");

            product.TransactionId = product.Transaction.Id;
            await _asyncRepository.UpdateAsync(product);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false, IndexingHelper.ProductSearchIndex(product),ElasticIndexConstant.PRODUCT_INDICES);
            
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update", "Product", product.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            var response = new ProductUpdateResponse();
            response.Product = product;
            return Ok(response);
        }
    }

}