using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Elasticsearch.Net;
using Infrastructure;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderSubmit : BaseAsyncEndpoint.WithRequest<ROSubmitRequest>.WithoutResponse
    {
        private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private IAsyncRepository<PurchaseOrder> _poAsyncRepository;
        private IAsyncRepository<PurchaseOrderSearchIndex> _poSearchIndexAsyncRepository;
        private IAsyncRepository<GoodsReceiptOrderSearchIndex> _roSearchIndexAsyncRepository;

        public ReceivingOrderSubmit(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository, IAsyncRepository<PurchaseOrderSearchIndex> poSearchIndexAsyncRepository, IAsyncRepository<GoodsReceiptOrderSearchIndex> roSearchIndexAsyncRepository)
        {
            _roAsyncRepository = roAsyncRepository;
            _poAsyncRepository = poAsyncRepository;
            _poSearchIndexAsyncRepository = poSearchIndexAsyncRepository;
            _roSearchIndexAsyncRepository = roSearchIndexAsyncRepository;
        }

        [HttpPost("api/goodsreceipt/submit")]
        [SwaggerOperation(
            Summary = "Submit Goods Receipt Order",
            Description = "Submit Goods Receipt Order",
            OperationId = "gr.submit",
            Tags = new[] { "GoodsReceiptOrders" })
        ]
        public override async Task<ActionResult> HandleAsync(ROSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var ro = await _roAsyncRepository.GetByIdAsync(request.ReceivingOrderId);
            var po = await _poAsyncRepository.GetByIdAsync(ro.PurchaseOrderId);
            ro.Transaction.ModifiedDate = DateTime.Now;
            po.PurchaseOrderStatus = PurchaseOrderStatusType.Done;
            
            
            await _poAsyncRepository.UpdateAsync(po);
            await _roAsyncRepository.UpdateAsync(ro);
            await _poSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po));
            await _roSearchIndexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.GoodsReceiptOrderSearchIndex(ro));
            return Ok();
        }
    }
}