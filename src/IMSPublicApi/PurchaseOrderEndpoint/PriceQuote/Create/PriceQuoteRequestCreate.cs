using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote.Create
{
    public class PriceQuoteRequestCreate : BaseAsyncEndpoint.WithRequest<PQCreateRequest>.WithResponse<PQCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<Product> _productRepos;

        public PriceQuoteRequestCreate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserAuthentication userAuthentication,  IAsyncRepository<Product> productRepos, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productRepos = productRepos;
            _indexAsyncRepository = indexAsyncRepository;
        }
        
        [HttpPost("api/pricequote/create/{Id}")]
        [SwaggerOperation(
            Summary = "Create price quote request with Id from purchase requisition",
            Description = "Create price quote request with Id from purchase requisition",
            OperationId = "po.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]

        public override async Task<ActionResult<PQCreateResponse>> HandleAsync([FromRoute]PQCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PriceQuoteOrder", UserOperations.Create))
                return Unauthorized();
            
            var response = new PQCreateResponse();
            var po = await _asyncRepository.GetByIdAsync(request.Id);
            var transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.PriceQuote,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                TransactionStatus = true
            };

            po.PurchaseOrderStatus = PurchaseOrderStatusType.PQCreated;
            po.Transaction = transaction;
            response.PurchaseOrderPQ = po;
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(true,IndexingHelper.PurchaseOrderSearchIndex(po));
            // pqr.CreatedBy
            return Ok(response);
        }
    }
}