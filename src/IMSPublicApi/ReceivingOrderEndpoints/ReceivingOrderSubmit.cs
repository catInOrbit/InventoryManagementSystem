using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderSubmit : BaseAsyncEndpoint.WithRequest<ROSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<ReceivingOrder> _roAsyncRepository;
        private IAsyncRepository<PurchaseOrder> _poAsyncRepository;

        public ReceivingOrderSubmit(IAsyncRepository<ReceivingOrder> roAsyncRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository)
        {
            _roAsyncRepository = roAsyncRepository;
            _poAsyncRepository = poAsyncRepository;
        }

        [HttpPost("api/receiving/submit")]
        [SwaggerOperation(
            Summary = "Finish Receiving Order",
            Description = "Finish Receiving Order",
            OperationId = "ro.submit",
            Tags = new[] { "ReceivingOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(ROSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var ro = await _roAsyncRepository.GetByIdAsync(request.ReceivingOrderId);
            var po = await _poAsyncRepository.GetByIdAsync(ro.PurchaseOrderId);
            ro.ModifiedDate = DateTime.Now;
            po.PurchaseOrderStatus = PurchaseOrderStatusType.Done;

            await _poAsyncRepository.UpdateAsync(po);
            await _roAsyncRepository.UpdateAsync(ro);
            return Ok();
        }
    }
}