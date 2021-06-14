using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.StockTakingEndpoints.Create;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.StockTakingEndpoints.Update
{
    public class StockTakeAddProduct : BaseAsyncEndpoint.WithRequest<STUpdateRequest>.WithResponse<STCreateItemResponse>
    {
        private readonly IAsyncRepository<StockTakeOrder> _stAsyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productAsyncRepository;
        private readonly IUserAuthentication _userAuthentication;

        public StockTakeAddProduct(IAsyncRepository<StockTakeOrder> stAsyncRepository, IAsyncRepository<ProductVariant> productAsyncRepository, IUserAuthentication userAuthentication)
        {
            _stAsyncRepository = stAsyncRepository;
            _productAsyncRepository = productAsyncRepository;
            _userAuthentication = userAuthentication;
        }
        
        [HttpPut("api/stocktake/add")]
        [SwaggerOperation(
            Summary = "Add product to check to stock take order",
            Description = "Add product to check to stock take order",
            OperationId = "st.add",
            Tags = new[] { "StockTakingEndpoints" })
        ]
        public override async Task<ActionResult<STCreateItemResponse>> HandleAsync(STUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new STCreateItemResponse();
            var stockTakeOrder = await _stAsyncRepository.GetByIdAsync(request.StockTakeId);
            stockTakeOrder.CheckItems = new List<StockTakeItem>();
            foreach (var stockTakeItem in request.StockTakeItems)
                stockTakeOrder.CheckItems.Add(stockTakeItem);
            
            stockTakeOrder.Transaction.ModifiedDate = DateTime.Now;
            stockTakeOrder.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            await _stAsyncRepository.UpdateAsync(stockTakeOrder);
            return Ok(response);
        }
    }
}