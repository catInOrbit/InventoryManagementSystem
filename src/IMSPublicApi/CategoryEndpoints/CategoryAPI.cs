using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using InventoryManagementSystem.PublicApi.ProductEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.CategoryEndpoints
{
    public class CategoryCreate : BaseAsyncEndpoint.WithRequest<CategoryCreateRequest>.WithoutResponse
    {
        private IAsyncRepository<Category> _categoryAsyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userSession;

        public CategoryCreate(IAsyncRepository<Category> categoryAsyncRepository, IAuthorizationService authorizationService, IUserSession userSession)
        {
            _categoryAsyncRepository = categoryAsyncRepository;
            _authorizationService = authorizationService;
            _userSession = userSession;
        }
        [HttpPost]
        [Route("api/category/create")]
        [SwaggerOperation(
            Summary = "Create a new category",
            Description = "Create a new category",
            OperationId = "category.create",
            Tags = new[] { "CategoryEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(CategoryCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.CATEGORY, UserOperations.Create))
                return Unauthorized();
            
            var category = new Category
            {
                CategoryName = request.CategoryName,
                CategoryDescription = request.CategoryDescription
            };
            category.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.Category, category.Id, (await _userSession.GetCurrentSessionUser()).Id);
            
            await _categoryAsyncRepository.AddAsync(category);
            await _categoryAsyncRepository.ElasticSaveSingleAsync(true, category,
                ElasticIndexConstant.CATEGORIES);
            return Ok();
        }
    }
    
    public class CategoryUpdate : BaseAsyncEndpoint.WithRequest<CategoryUpdateRequest>.WithResponse<CategoryUpdateResponse>
    {
        private IAsyncRepository<Category> _categoryAsyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;

        public CategoryUpdate(IAsyncRepository<Category> categoryAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication)
        {
            _categoryAsyncRepository = categoryAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
        }
        [HttpPut]
        [Route("api/category/update")]
        [SwaggerOperation(
            Summary = "Update a category",
            Description = "Update a category",
            OperationId = "category.create",
            Tags = new[] { "CategoryEndpoints" })
        ]

        public override async Task<ActionResult<CategoryUpdateResponse>> HandleAsync(CategoryUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.CATEGORY, UserOperations.Create))
                return Unauthorized();
            var category = await _categoryAsyncRepository.GetByIdAsync(request.CategoryId);
            CategoryUpdateResponse response = null;
            if (category == null)
            {
                response = new CategoryUpdateResponse
                {
                    Status = false,
                    Verbose = "Can not find category ID"
                };

                return NotFound(response);
            }

            category.Transaction = TransactionUpdateHelper.UpdateTransaction(category.Transaction,UserTransactionActionType.Modify ,category.Id,
                (await _userAuthentication.GetCurrentSessionUser()).Id);
            
            category.CategoryName = request.CategoryName;
            category.CategoryDescription = request.CategoryDescription;
            
            await _categoryAsyncRepository.UpdateAsync(category);
            await _categoryAsyncRepository.ElasticSaveSingleAsync(false, category,
                ElasticIndexConstant.CATEGORIES);
            
            
            return Ok();
        }
    }
    
    public class GetCategoryById : BaseAsyncEndpoint.WithRequest<CategorySearchIdRequest>.WithResponse<CategorySearchIdResponse>
    {
        private IAsyncRepository<Category> _categoryAsyncRepository;

        public GetCategoryById(IAsyncRepository<Category> categoryAsyncRepository)
        {
            _categoryAsyncRepository = categoryAsyncRepository;
        }
        [HttpGet]
        [Route("api/category/{Id}")]
        [SwaggerOperation(
            Summary = "Search a specific category",
            Description = "Create a new category",
            OperationId = "category.create",
            Tags = new[] { "CategoryEndpoints" })
        ]

        public override async Task<ActionResult<CategorySearchIdResponse>> HandleAsync([FromRoute]CategorySearchIdRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new CategorySearchIdResponse();
            response.Category = await _categoryAsyncRepository.GetByIdAsync(request.Id);
            return Ok(response);
        }
    }
    
    public class GetAllCategory : BaseAsyncEndpoint.WithRequest<GetCategoryRequest>.WithResponse<GetAllCategoryResponse>
    {
        private IAsyncRepository<Category> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetAllCategory(IAsyncRepository<Category> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet("api/category")]
        [SwaggerOperation(
            Summary = "Get list of category",
            Description = "Get list of category" +
                          "\n{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page",
            OperationId = "product-category.getall",
            Tags = new[] { "CategoryEndpoints" })
        ]
        public override async Task<ActionResult<GetAllCategoryResponse>> HandleAsync([FromQuery] GetCategoryRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.CATEGORY, UserOperations.Read))
                return Unauthorized();
            
            var response = new GetAllCategoryResponse();
            response.IsDisplayingAll = true;
            PagingOption<Category> pagingOption = new PagingOption<Category>(
                request.CurrentPage, request.SizePerPage);
            response.Paging = (await _asyncRepository.ListAllAsync(pagingOption, cancellationToken));
            return Ok(response);
        }
    }
}