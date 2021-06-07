using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.GoodsReceiptEndpoints.Create
{
    public class GoodsIssueCreate : BaseAsyncEndpoint.WithRequest<GRCreateRequest>.WithResponse<GRCreateResponse>
    {
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GoodsIssueCreate(IUserAuthentication userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpPost("api/goodsissue/create")]
        [SwaggerOperation(
            Summary = "Create Good issue order",
            Description = "Create Good issue order",
            OperationId = "gio.create",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        public override async Task<ActionResult<GRCreateResponse>> HandleAsync(GRCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GRCreateResponse();
            
            var gio = _asyncRepository.get
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PriceQuoteOrder", UserOperations.Create))
                return Unauthorized();
            
            var gio = new GoodsIssueOrder();
            gio.Transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.PriceQuote,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
            };
            gio.GoodsIssueType = GoodsIssueType.Packing; 
            response.GoodsIssueOrder = gio;
            await _asyncRepository.AddAsync(gio);
            return Ok(response);
        }
    }
}