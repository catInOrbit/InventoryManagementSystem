using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using InventoryManagementSystem.PublicApi.ProductEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.CategoryEndpoints
{
    public class CategoryCreate : BaseAsyncEndpoint.WithRequest<CategoryCreateRequest>.WithoutResponse
    {
        private IAsyncRepository<Category> _categoryAsyncRepository;
        private IAsyncRepository<CategorySearchIndex> _categoryIndexAsyncRepository;
        private IElasticAsyncRepository<Category> _categoryEls;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userSession;

        public CategoryCreate(IAsyncRepository<Category> categoryAsyncRepository, IAsyncRepository<CategorySearchIndex> categoryIndexAsyncRepository, IAuthorizationService authorizationService, IUserSession userSession, IElasticAsyncRepository<Category> categoryEls)
        {
            _categoryAsyncRepository = categoryAsyncRepository;
            _categoryIndexAsyncRepository = categoryIndexAsyncRepository;
            _authorizationService = authorizationService;
            _userSession = userSession;
            _categoryEls = categoryEls;
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
                CategoryDescription = request.CategoryDescription,
                LatestUpdateDate = DateTime.UtcNow,
            };
            
            category.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.Category, category.Id, (await _userSession.GetCurrentSessionUser()).Id);
            
            await _categoryAsyncRepository.AddAsync(category);
            await _categoryEls.ElasticSaveSingleAsync(false, category,
                ElasticIndexConstant.CATEGORIES);
            return Ok();
        }
    }
    
    public class CategoryUpdate : BaseAsyncEndpoint.WithRequest<CategoryUpdateRequest>.WithResponse<CategoryUpdateResponse>
    {
        private IAsyncRepository<Category> _categoryAsyncRepository;
        private IAsyncRepository<CategorySearchIndex> _categoryIndexAsyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private IElasticAsyncRepository<Category> _categoryEls;

        public CategoryUpdate(IAsyncRepository<Category> categoryAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, IAsyncRepository<CategorySearchIndex> categoryIndexAsyncRepository, IElasticAsyncRepository<Category> categoryEls)
        {
            _categoryAsyncRepository = categoryAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _categoryIndexAsyncRepository = categoryIndexAsyncRepository;
            _categoryEls = categoryEls;
        }
        [HttpPut]
        [Route("api/category/update")]
        [SwaggerOperation(
            Summary = "Update a category",
            Description = "Update a category",
            OperationId = "category.update",
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

            category.Transaction = TransactionUpdateHelper.UpdateTransaction(category.Transaction,UserTransactionActionType.Modify,TransactionType.Category,
                (await _userAuthentication.GetCurrentSessionUser()).Id, category.Id, "");
            
            category.CategoryName = request.CategoryName;
            category.CategoryDescription = request.CategoryDescription;
            category.LatestUpdateDate = DateTime.UtcNow;

            await _categoryAsyncRepository.UpdateAsync(category);
            
            await _categoryEls.ElasticSaveSingleAsync(false, category,
                ElasticIndexConstant.CATEGORIES);
            response = new CategoryUpdateResponse
            {
                Status = true,
                Verbose = "Updated category",
                UpdatedCategory = category
            };
            
            return Ok(response);
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
            OperationId = "category.searchid",
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
        private IElasticClient _elasticClient;
        
        public GetAllCategory(IAsyncRepository<Category> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }
        
        [HttpGet("api/category")]
        [SwaggerOperation(
            Summary = "Get list of category",
            Description = "Get list of category" +
                          "\n{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page",
            OperationId = "product_category.searchall",
            Tags = new[] { "CategoryEndpoints" })
        ]
        public override async Task<ActionResult<GetAllCategoryResponse>> HandleAsync([FromQuery] GetCategoryRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.CATEGORY, UserOperations.Read))
                return Unauthorized();
            
            var response = new GetAllCategoryResponse();
            PagingOption<Category> pagingOption = new PagingOption<Category>(
                request.CurrentPage, request.SizePerPage);
            response.IsDisplayingAll = true;
            
            ISearchResponse<Category> responseElastic;

            ElasticSearchHelper<Category> elasticSearchHelper = new ElasticSearchHelper<Category>(_elasticClient, request.SearchQuery,
                ElasticIndexConstant.CATEGORIES);
            responseElastic = await elasticSearchHelper.GetDocuments();
            
            pagingOption.ResultList = responseElastic.Documents.ToList();
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
}