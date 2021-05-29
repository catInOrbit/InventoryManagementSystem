using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class GetProductById : BaseAsyncEndpoint.WithRequest<GetProductRequest>.WithoutResponse
    {
        private IAsyncRepository<Product> _asyncRepository;

        public GetProductById(IAsyncRepository<Product> asyncRepository)
        {
            _asyncRepository = asyncRepository;
        }

        [HttpGet("api/product/{ProductId}")]
        [SwaggerOperation(
            Summary = "Update purchase order",
            Description = "Update purchase order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] GetProductRequest request, CancellationToken cancellationToken = new CancellationToken())
        {   
            var response = new GetProductResponse();
            response.Product = await _asyncRepository.GetByIdAsync(request.ProductId, cancellationToken);

            return Ok(response);
        }
    }
}