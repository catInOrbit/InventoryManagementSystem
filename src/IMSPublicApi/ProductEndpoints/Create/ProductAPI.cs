using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Create
{
    public class ProductCreate : BaseAsyncEndpoint.WithRequest<ProductRequest>.WithResponse<ProductCreateResponse>
    {
        private IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private IAsyncRepository<Category> _categoryAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;
        private readonly IAsyncRepository<ProductVariantSearchIndex> _productVariantIndexAsyncRepositoryRepos;

        private readonly INotificationService _notificationService;

        public ProductCreate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos, INotificationService notificationService, IAsyncRepository<ProductVariantSearchIndex> productVariantIndexAsyncRepositoryRepos, IAsyncRepository<Category> categoryAsyncRepository)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
            _productVariantIndexAsyncRepositoryRepos = productVariantIndexAsyncRepositoryRepos;
            _categoryAsyncRepository = categoryAsyncRepository;
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
               
            var category = await _categoryAsyncRepository.GetByIdAsync(request.CategoryId);

            ApplicationCore.Entities.Products.Product product = new ApplicationCore.Entities.Products.Product
            {
                Name = request.Name,
                Brand = new Brand
                {
                    BrandDescription = request.BrandDescription,
                    BrandName = request.BrandName
                },
                CategoryId = request.CategoryId,
                Category = category,
                Unit = request.Unit
            };

            product.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.Product,
                product.Id, (await _userAuthentication.GetCurrentSessionUser()));


            product.TransactionId = product.Transaction.Id;
            product.IsVariantType = request.IsVariantType;
            if(request.ProductImageLink!=null) product.ProductImageLink = request.ProductImageLink;
            product.ProductVariants = new List<ProductVariant>();
            foreach (var productVairantRequestInfo in request.ProductVariants)
            {
                var productVariant = new ProductVariant
                {
                    Name = productVairantRequestInfo.Name,
                    Sku = productVairantRequestInfo.Sku,
                    IsVariantType = product.IsVariantType,
                    Barcode = productVairantRequestInfo.Barcode,
                    Price = productVairantRequestInfo.Price,
                    Cost = productVairantRequestInfo.SalePrice,  
                    Product =  product
                    // Unit = product.Unit
                };
                productVariant.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.ProductVariant, productVariant.Id, 
                    (await _userAuthentication.GetCurrentSessionUser()));

                var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);
                productVariant.Packages = new List<Package>();
                foreach (var package in packages)
                {
                    productVariant.StorageQuantity += package.Quantity;
                    productVariant.Packages.Add(package);
                }
                

                await _productVariantIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,
                    IndexingHelper.ProductVariantSearchIndex(productVariant),
                    ElasticIndexConstant.PRODUCT_VARIANT_INDICES);

                product.ProductVariants.Add(productVariant);
            }

            // if (!product.IsVariantType)
            //     product.Name = product.ProductVariants.ToList()[0].Name;
            
            await _asyncRepository.AddAsync(product);
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(true,IndexingHelper.ProductSearchIndex(product),ElasticIndexConstant.PRODUCT_INDICES);
            
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

        private IElasticClient _elasticClient;

        public ProductVariantUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductVariantSearchIndex> productVariantIndexAsyncRepositoryRepos, INotificationService notificationService, IRedisRepository redisRepository, IAsyncRepository<Package> pacakgeAsyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos, IElasticClient elasticClient)
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
            _elasticClient = elasticClient;
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
            bool productWasVariantType = product.IsVariantType;
            product.IsVariantType = request.IsVariantType;
            
            var listNewVariant = new List<ProductVairantUpdateRequestInfo>(request.ProductVariantsUpdate);
            ElasticSearchHelper<ProductVariantSearchIndex> elasticSearchHelper =
                new ElasticSearchHelper<ProductVariantSearchIndex>(_elasticClient);

            if (product.IsVariantType)
            {
                if (!productWasVariantType)
                {
                    //Purge First index
                    foreach (var productProductVariant in product.ProductVariants)
                    {
                        productProductVariant.Transaction.TransactionStatus = false;
                        await _productVariantIndexAsyncRepositoryRepos.ElasticDeleteSingleAsync(IndexingHelper.ProductVariantSearchIndex(productProductVariant),ElasticIndexConstant.PRODUCT_VARIANT_INDICES);

                        productProductVariant.Transaction.Type = TransactionType.Deleted;
                        await _productVariantAsyncRepository.UpdateAsync(productProductVariant);
                    }
                    product.ProductVariants.Clear();
                }
                                
                foreach (var productVairantRequestInfo in request.ProductVariantsUpdate)
                {
                    foreach (var productVariantSystemList in product.ProductVariants)
                    {
                        if (productVairantRequestInfo.Id != null && productVariantSystemList.Id == productVairantRequestInfo.Id)
                        {
                            // var responseElasticCheck = await elasticSearchHelper.CheckFieldExistProduct(productVairantRequestInfo.Name, productVairantRequestInfo.Sku);
                            productVariantSystemList.Name = productVairantRequestInfo.Name;
                            productVariantSystemList.Sku = productVairantRequestInfo.Sku;
                            productVariantSystemList.IsVariantType = product.IsVariantType;
                            productVariantSystemList.Barcode = productVairantRequestInfo.Barcode;
                            productVariantSystemList.Price = productVairantRequestInfo.Price;

                            if (productVairantRequestInfo.ProductVariantImageLink != null)
                                productVariantSystemList.VariantImageLink =
                                    productVairantRequestInfo.ProductVariantImageLink;

                            
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
                        IsVariantType = product.IsVariantType,
                        Barcode = productVairantRequestInfo.Barcode,
                        Price = productVairantRequestInfo.Price,
                        ProductId = product.Id,
                    };
                    
                    
                    if (productVairantRequestInfo.ProductVariantImageLink != null)
                        productVariant.VariantImageLink =
                            productVairantRequestInfo.ProductVariantImageLink;
                    
                    productVariant.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.ProductVariant, productVariant.Id, 
                        (await _userAuthentication.GetCurrentSessionUser()).Id);
                        
                    var packages = await _asyncRepository.GetPackagesFromProductVariantId(productVariant.Id);
                    
                    // ElasticSearchHelper<ProductVariantSearchIndex> elasticSearchHelper = new ElasticSearchHelper<ProductVariantSearchIndex>()

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
                //Purge All index
                foreach (var productProductVariant in product.ProductVariants)
                {
                    productProductVariant.Transaction.TransactionStatus = false;
                    await _productVariantIndexAsyncRepositoryRepos.ElasticDeleteSingleAsync(IndexingHelper.ProductVariantSearchIndex(productProductVariant),ElasticIndexConstant.PRODUCT_VARIANT_INDICES);

                    productProductVariant.Transaction.Type = TransactionType.Deleted;
                    await _productVariantAsyncRepository.UpdateAsync(productProductVariant);
                }
                product.ProductVariants.Clear();
                
                var productVairantUpdateRequestInfo = request.ProductVariantsUpdate[0];
                
                var productVariant = new ProductVariant
                {
                    Name = productVairantUpdateRequestInfo.Name,
                    Sku = productVairantUpdateRequestInfo.Sku,
                    IsVariantType = product.IsVariantType,
                    Barcode = productVairantUpdateRequestInfo.Barcode,
                    Price = productVairantUpdateRequestInfo.Price,
                    ProductId = product.Id,
                    Product = product
                };
                
                if (productVairantUpdateRequestInfo.ProductVariantImageLink != null)
                    productVariant.VariantImageLink =
                        productVairantUpdateRequestInfo.ProductVariantImageLink;

                
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


            // await _asyncRepository.ReIndexProduct();
            // await _asyncRepository.ReIndexProductVariant();

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
           product.Unit = request.Unit;

           if (request.ProductImageLink != null) product.ProductImageLink = product.ProductImageLink;
           
           // foreach (var productProductVariant in product.ProductVariants)
           //     productProductVariant.Unit = product.Unit;
           
           product.Transaction = TransactionUpdateHelper.UpdateTransaction(product.Transaction,UserTransactionActionType.Modify,
                (await _userAuthentication.GetCurrentSessionUser()).Id, product.Id, "");

            product.TransactionId = product.Transaction.Id;
            await _asyncRepository.UpdateAsync(product);
            
      
            
            await _productIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false, IndexingHelper.ProductSearchIndex(product),ElasticIndexConstant.PRODUCT_INDICES);
            
            // await _asyncRepository.ReIndexProduct();

            var response = new ProductUpdateResponse();
            response.Product = product;
            return Ok(response);
        }
    }
    
    public class ProductImageUpdate : BaseAsyncEndpoint.WithRequest<ProductImageRequest>.WithoutResponse
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Products.Product> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly IAsyncRepository<ProductSearchIndex> _productIndexAsyncRepositoryRepos;
        private readonly IAsyncRepository<ProductVariantSearchIndex> _productVariantIndexAsyncRepositoryRepos;
        private readonly INotificationService _notificationService;

        public ProductImageUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Product> asyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<ProductSearchIndex> productIndexAsyncRepositoryRepos, IAsyncRepository<ProductVariantSearchIndex> productVariantIndexAsyncRepositoryRepos, INotificationService notificationService)
        {
            _asyncRepository = asyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _productIndexAsyncRepositoryRepos = productIndexAsyncRepositoryRepos;
            _productVariantIndexAsyncRepositoryRepos = productVariantIndexAsyncRepositoryRepos;
            _notificationService = notificationService;
        }

        [HttpPut("api/productglobal/updateimage")]
        [SwaggerOperation(
            Summary = "Update picture product (productVariant)",
            Description = "Update picture product (productVariant)",
            OperationId = "product.pic",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ProductImageRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
           if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Update))
            return Unauthorized();

           var product = await _asyncRepository.GetByIdAsync(request.Id);
           var productVariant = await _productVariantAsyncRepository.GetByIdAsync(request.Id);

           if (product != null)
           {
               product.ProductImageLink = request.ImageLink;
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
               return Ok();
           }

           if (productVariant != null)
           {
               productVariant.VariantImageLink = request.ImageLink;
               productVariant.Transaction = TransactionUpdateHelper.UpdateTransaction(productVariant.Transaction,UserTransactionActionType.Modify,
                   (await _userAuthentication.GetCurrentSessionUser()).Id, productVariant.Id, "");
               productVariant.TransactionId = productVariant.Transaction.Id;
               await _productVariantAsyncRepository.UpdateAsync(productVariant);
            
               await _productVariantIndexAsyncRepositoryRepos.ElasticSaveSingleAsync(false, IndexingHelper.ProductVariantSearchIndex(productVariant),ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
               return Ok();
           }
           
           
           return NotFound("Can not found product / product variant with id : "+request.Id);
        }
    }

}
