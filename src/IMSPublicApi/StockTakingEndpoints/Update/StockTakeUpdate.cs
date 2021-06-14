using System;
using System.Linq;
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

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Update
{
    public class StockTakeUpdate : BaseAsyncEndpoint.WithRequest<STUpdateRequest>.WithoutResponse
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserAuthentication _userAuthentication;

        public StockTakeUpdate(IAsyncRepository<StockTakeOrder> productAsyncRepository, IAuthorizationService authorizationService, IUserAuthentication userAuthentication)
        {
            _asyncRepository = productAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
        }

        [HttpPut("api/stocktake/update")]
        [SwaggerOperation(
            Summary = "Update stock taking file",
            Description = "Update stock taking file",
            OperationId = "st.update",
            Tags = new[] { "StockTakingEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(STUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "StockTaking", UserOperations.Update))
                return Unauthorized();

            var storder = await _asyncRepository.GetByIdAsync(request.StockTakeId);
            foreach (var requestStockTakeItem in request.StockTakeItems)
            {
                if (storder.CheckItems.Any(item => item.Id == requestStockTakeItem.Id))
                {
                    storder.CheckItems.Remove(requestStockTakeItem);
                    storder.CheckItems.Add(requestStockTakeItem);
                }
            }

            storder.Transaction.ModifiedDate = DateTime.Now;
            storder.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            await _asyncRepository.UpdateAsync(storder);
            return Ok();
        }
    }
}