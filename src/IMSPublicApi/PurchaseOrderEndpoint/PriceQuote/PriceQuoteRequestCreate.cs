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

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteRequestCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<PQCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<PriceQuoteOrder> _asyncRepository;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<Product> _productRepos;

        public PriceQuoteRequestCreate(IAuthorizationService authorizationService, IAsyncRepository<PriceQuoteOrder> asyncRepository, IUserAuthentication userAuthentication,  IAsyncRepository<Product> productRepos)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _productRepos = productRepos;
        }
        
        [HttpPost("api/pricequote/create")]
        [SwaggerOperation(
            Summary = "Create price quote request",
            Description = "Create price quote request",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult<PQCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PriceQuoteOrder",
                UserOperations.Create);

            if (!isAuthorized.Succeeded)
                return Unauthorized();
            
            var response = new PQCreateResponse();
            var pqr = new PriceQuoteOrder();
            pqr.CreatedById =  (await _userAuthentication.GetCurrentSessionUser()).Id;
            response.PriceQuoteOrder = pqr;
            await _asyncRepository.AddAsync(pqr);
            // pqr.CreatedBy
            return Ok(response);
        }
    }
}