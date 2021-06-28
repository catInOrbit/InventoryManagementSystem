using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.CategoryEndpoints
{
    public class CategoryCreate : BaseAsyncEndpoint.WithRequest<CategoryCreateRequest>.WithoutResponse
    {
        private IAsyncRepository<Category> _categoryAsyncRepository;

        public CategoryCreate(IAsyncRepository<Category> categoryAsyncRepository)
        {
            _categoryAsyncRepository = categoryAsyncRepository;
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
            await _categoryAsyncRepository.AddAsync(request.Category);
            await _categoryAsyncRepository.ElasticSaveSingleAsync(true, request.Category,
                ElasticIndexConstant.CATEGORIES);
            return Ok();
        }
    }
    
    public class CategoryUpdate : BaseAsyncEndpoint.WithRequest<CategoryUpdateRequest>.WithResponse<CategoryUpdateResponse>
    {
        private IAsyncRepository<Category> _categoryAsyncRepository;

        public CategoryUpdate(IAsyncRepository<Category> categoryAsyncRepository)
        {
            _categoryAsyncRepository = categoryAsyncRepository;
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

            category.CategoryName = request.CategoryUpdateInfo.CategoryName;
            category.CategoryDescription = request.CategoryUpdateInfo.CategoryDescription;
            
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
}