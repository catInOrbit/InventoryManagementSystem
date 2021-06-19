using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.SupplierSearch
{
    public class GetAllSupplier : BaseAsyncEndpoint.WithRequest<GetAllSupplierRequest>.WithResponse<GetSupplierResponse>
    {
        private IAsyncRepository<Supplier> _asyncRepository;
        private readonly IElasticClient _elasticClient;

        public GetAllSupplier(IAsyncRepository<Supplier> asyncRepository, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _elasticClient = elasticClient;
        }
        
        [HttpPost("api/suppliers/all")]
        [SwaggerOperation(
            Summary = "Get all supplier",
            Description = "Get all supplier"  +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page \n " +
                          "{Status} Status of purchase order",
            OperationId = "sups.update",
            Tags = new[] { "SupplierEndpoints" })
        ]
        public override async Task<ActionResult<GetSupplierResponse>> HandleAsync(GetAllSupplierRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetSupplierResponse();
            PagingOption<Supplier> pagingOption =
                new PagingOption<Supplier>(request.CurrentPage, request.SizePerPage);
            response.IsDisplayingAll = true;
            
            response.Paging = await 
                _asyncRepository.ListAllAsync(pagingOption, cancellationToken);
     
            return Ok(response);
        }
    }
    
     public class SearchSupplier : BaseAsyncEndpoint.WithRequest<SupplierSearchRequest>.WithResponse<GetSupplierResponse>
    {
        private IAsyncRepository<Supplier> _asyncRepository;
        private readonly IElasticClient _elasticClient;

        public SearchSupplier(IAsyncRepository<Supplier> asyncRepository, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _elasticClient = elasticClient;
        }
        
        [HttpPost("api/suppliers/search")]
        [SwaggerOperation(
            Summary = "Get all supplier",
            Description = "Get all supplier"  +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page \n " +
                          "{Status} Status of purchase order",
            OperationId = "sups.update",
            Tags = new[] { "SupplierEndpoints" })
        ]
        public override async Task<ActionResult<GetSupplierResponse>> HandleAsync(SupplierSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetSupplierResponse();
            PagingOption<Supplier> pagingOption =
                new PagingOption<Supplier>(request.CurrentPage, request.SizePerPage);
            response.IsDisplayingAll = true;

            var responseElastic = await _elasticClient.SearchAsync<Supplier>
            (
                s => s.Index(ElasticIndexConstant.SUPPLIERS).Query(q => q.QueryString(d => d.Query('*' + request.Query + '*'))));

            foreach (var purchaseOrderSearchIndex in responseElastic.Documents)
            {
                pagingOption.ResultList.Add(purchaseOrderSearchIndex);
            }
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
}