using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UtilEndpoints
{
    public class DuplicateProductChecker : BaseAsyncEndpoint.WithRequest<DuplicateCheckerRequest>.WithResponse<DuplicateCheckerResponse>
    {
        private readonly IElasticClient _elasticClient;

        public DuplicateProductChecker(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        [HttpPost("api/dupcheck/product")]
        [SwaggerOperation(
            Summary = "Check product field for duplication (Name)",
            Description = "Check product field for duplication (Name)",
            OperationId = "product.searchvariants",
            Tags = new[] { "UtilsEndpoints" })
        ]
        public override async Task<ActionResult<DuplicateCheckerResponse>> HandleAsync(DuplicateCheckerRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new DuplicateCheckerResponse();
            ElasticSearchHelper<ProductSearchIndex> elasticSearchHelper =
                new ElasticSearchHelper<ProductSearchIndex>(_elasticClient);

            var responseElastic = await elasticSearchHelper.CheckFieldExistProduct(request.Value);
            if (responseElastic.Documents.Count > 0)
            {
                response.HasMatch = true;
                response.MatchList.AddRange(responseElastic.Documents.GroupBy(x => x.Name).Select(x=> x.First()));
            }
            return Ok(response);
        }
    }
    
    public class DuplicateProductVariantChecker : BaseAsyncEndpoint.WithRequest<DuplicateCheckerRequest>.WithResponse<DuplicateCheckerResponse>
    {
        private readonly IElasticClient _elasticClient;
        public DuplicateProductVariantChecker(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        
        [HttpPost("api/dupcheck/productvariant")]
        [SwaggerOperation(
            Summary = "Check product variant field for duplication (Name, SKU, Barcode)",
            Description = "Check product variant field for duplication (Name, SKU, Barcode)",
            OperationId = "product.searchvariants",
            Tags = new[] { "UtilsEndpoints" })
        ]
        public override async Task<ActionResult<DuplicateCheckerResponse>> HandleAsync(DuplicateCheckerRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new DuplicateCheckerResponse();
            ElasticSearchHelper<ProductVariantSearchIndex> elasticSearchHelper =
                new ElasticSearchHelper<ProductVariantSearchIndex>(_elasticClient);

            var responseElastic = await elasticSearchHelper.CheckFieldExistProductVariant(request.Value);
            if (responseElastic.Documents.Count > 0)
            {
                response.HasMatch = true;
                response.MatchList.AddRange(responseElastic.Documents.GroupBy(x => x.Name).Select(x=> x.First()));
            }
            return Ok(response);
        }
    }
    
    public class DuplicateLocationChecker : BaseAsyncEndpoint.WithRequest<DuplicateCheckerRequest>.WithResponse<DuplicateCheckerResponse>
    {
        private readonly IElasticClient _elasticClient;
        public DuplicateLocationChecker(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        
        [HttpPost("api/dupcheck/location")]
        [SwaggerOperation(
            Summary = "Check location field for duplication (Location Name)",
            Description = "Check location field for duplication (Location Name)",
            OperationId = "product.searchvariants",
            Tags = new[] { "UtilsEndpoints" })
        ]
        public override async Task<ActionResult<DuplicateCheckerResponse>> HandleAsync(DuplicateCheckerRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new DuplicateCheckerResponse();
            ElasticSearchHelper<Location> elasticSearchHelper =
                new ElasticSearchHelper<Location>(_elasticClient);

            var responseElastic = await elasticSearchHelper.CheckFieldExistLocation(request.Value);
            if (responseElastic.Documents.Count > 0)
            {
                response.HasMatch = true;
                response.MatchList.AddRange(responseElastic.Documents.GroupBy(x => x.LocationName).Select(x=> x.First()));
            }
            return Ok(response);
        }
    }
    
    public class DuplicateCategoryChecker : BaseAsyncEndpoint.WithRequest<DuplicateCheckerRequest>.WithResponse<DuplicateCheckerResponse>
    {
        private readonly IElasticClient _elasticClient;
        public DuplicateCategoryChecker(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        
        [HttpPost("api/dupcheck/category")]
        [SwaggerOperation(
            Summary = "Check category field for duplication (Category Name)",
            Description = "Check category field for duplication (Category Name)",
            OperationId = "product.searchvariants",
            Tags = new[] { "UtilsEndpoints" })
        ]
        public override async Task<ActionResult<DuplicateCheckerResponse>> HandleAsync(DuplicateCheckerRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new DuplicateCheckerResponse();
            ElasticSearchHelper<Category> elasticSearchHelper =
                new ElasticSearchHelper<Category>(_elasticClient);

            var responseElastic = await elasticSearchHelper.CheckFieldExistCategory(request.Value);
            if (responseElastic.Documents.Count > 0)
            {
                response.HasMatch = true;
                response.MatchList.AddRange(responseElastic.Documents.GroupBy(x => x.CategoryName).Select(x=> x.First()));
            }
            return Ok(response);
        }
    }
    
    public class DuplicateSupplierChecker : BaseAsyncEndpoint.WithRequest<DuplicateCheckerRequest>.WithResponse<DuplicateCheckerResponse>
    {
        private readonly IElasticClient _elasticClient;
        public DuplicateSupplierChecker(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        
        [HttpPost("api/dupcheck/supplier")]
        [SwaggerOperation(
            Summary = "Check category field for duplication (Name, Email)",
            Description = "Check category field for duplication (Name, Email)",
            OperationId = "product.searchvariants",
            Tags = new[] { "UtilsEndpoints" })
        ]
        public override async Task<ActionResult<DuplicateCheckerResponse>> HandleAsync(DuplicateCheckerRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new DuplicateCheckerResponse();
            ElasticSearchHelper<Supplier> elasticSearchHelper =
                new ElasticSearchHelper<Supplier>(_elasticClient);

            var responseElastic = await elasticSearchHelper.CheckFieldExistSupplier(request.Value);
            if (responseElastic.Documents.Count > 0)
            {
                response.HasMatch = true;
                response.MatchList.AddRange(responseElastic.Documents.GroupBy(x => x.SupplierName).Select(x=> x.First()));
            }
            return Ok(response);
        }
    }
}