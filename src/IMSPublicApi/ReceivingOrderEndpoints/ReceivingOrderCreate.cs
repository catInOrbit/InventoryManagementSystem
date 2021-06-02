using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<ROCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ReceivingOrder> _receiveAsyncRepository;

  
        public ReceivingOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<ReceivingOrder> receiveAsyncRepository)
        {
            _authorizationService = authorizationService;
            _receiveAsyncRepository = receiveAsyncRepository;
        }
        
              
        [HttpPost("api/receiving/create")]
        [SwaggerOperation(
            Summary = "Create G",
            Description = "Authenticates a user",
            OperationId = "auth.authenticate",
            Tags = new[] { "IMSAuthenticationEndpoints" })
        ]
        public override async Task<ActionResult<ROCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            var ro = new ReceivingOrder();
            await _receiveAsyncRepository.AddAsync(ro);
            var response = new ROCreateResponse();
            response.ReceivingOrder = ro;
            return Ok(response);
        }
    }
}