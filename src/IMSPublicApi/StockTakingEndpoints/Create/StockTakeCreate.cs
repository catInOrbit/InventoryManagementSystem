using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Create
{
    public class StockTakeCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<STCreateItemResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IUserAuthentication _userAuthentication;

        public StockTakeCreate(IAuthorizationService authorizationService, IAsyncRepository<StockTakeOrder> asyncRepository, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
        }

        [HttpPost("api/stocktake/create")]
        [SwaggerOperation(
            Summary = "Create a stock take order",
            Description = "Create a stock take order",
            OperationId = "st.create",
            Tags = new[] { "StockTakingEndpoints" })
        ]
        public override async Task<ActionResult<STCreateItemResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "StockTakeOrder", UserOperations.Create))
                return Unauthorized();

            var response = new STCreateItemResponse();

            var sto = new StockTakeOrder
            {
                Transaction = new Transaction
                {
                    CreatedDate = DateTime.Now,
                    Type = TransactionType.StockTake,
                    CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
                }
            };

            await _asyncRepository.AddAsync(sto);
            return Ok();
        }
    }
} 