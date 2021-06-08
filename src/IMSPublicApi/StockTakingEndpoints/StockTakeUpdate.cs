using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints
{
    public class StockTakeUpdate : BaseAsyncEndpoint.WithRequest<STUpdateRequest>.WithoutResponse
    {
        private readonly IAsyncRepository<StockTakeOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public StockTakeUpdate(IAsyncRepository<StockTakeOrder> productAsyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = productAsyncRepository;
            _authorizationService = authorizationService;
        }

        public override async Task<ActionResult> HandleAsync(STUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "StockTaking", UserOperations.Update))
                return Unauthorized();

            var storder = await _asyncRepository.GetByIdAsync(request.StockTakeId);
            storder.Note = request.Note;
            storder.RecordedQuantity = request.RecordedQuantity;

            await _asyncRepository.UpdateAsync(storder);
            return Ok();
        }
    }
}