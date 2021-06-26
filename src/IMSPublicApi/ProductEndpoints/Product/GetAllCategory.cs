using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Product
{
    public class GetAllCategory : BaseAsyncEndpoint.WithRequest<GetCategoryRequest>.WithResponse<GetAllCategoryResponse>
    {
        private IAsyncRepository<Category> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetAllCategory(IAsyncRepository<Category> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet("api/product/category")]
        [SwaggerOperation(
            Summary = "Get list of category",
            Description = "Get list of category" +
                          "\n{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page",
            OperationId = "product-category.getall",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<GetAllCategoryResponse>> HandleAsync([FromQuery] GetCategoryRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Read))
                return Unauthorized();
            
            var response = new GetAllCategoryResponse();
            PagingOption<Category> pagingOption = new PagingOption<Category>(
                request.CurrentPage, request.SizePerPage);
            
            var list = (await _asyncRepository.ListAllAsync(pagingOption, cancellationToken));
            response.Categories = list.ResultList.ToList();
            return Ok(response);
        }
    }
}