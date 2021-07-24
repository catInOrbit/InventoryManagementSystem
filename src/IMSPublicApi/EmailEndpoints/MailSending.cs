using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Castle.Core.Internal;
using Infrastructure;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.EmailEndpoints
{

    public class SendMail : BaseAsyncEndpoint.WithRequest<MailSendingRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;
        private IAsyncRepository<PurchaseOrder> _purchaseOrderAsyncRepository;
        private IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;

        public SendMail(IEmailSender emailSender, IAsyncRepository<PurchaseOrder> purchaseOrderAsyncRepository, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository)
        {
            _emailSender = emailSender;
            _purchaseOrderAsyncRepository = purchaseOrderAsyncRepository;
            _indexAsyncRepository = indexAsyncRepository;
        }

        [HttpPost("api/mailservice")]
        [SwaggerOperation(
            Summary = "Send email to specified address",
            Description = "Send email to specified address",
            OperationId = "mail",
            Tags = new[] { "MailEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromForm] MailSendingRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            var message = new EmailMessage(request.To, request.Subject, request.Content, files);

            if (!request.PurchaseOrderId.IsNullOrEmpty())
            {
                try
                {
                    var po = await _purchaseOrderAsyncRepository.GetByIdAsync(request.PurchaseOrderId);
                    po.HasSentMail = true;
                    
                    await _purchaseOrderAsyncRepository.UpdateAsync(po);
                    await _indexAsyncRepository.ElasticSaveSingleAsync(false,
                        IndexingHelper.PurchaseOrderSearchIndex(po), ElasticIndexConstant.PURCHASE_ORDERS);

                    var response = new MailSendingPOResponse();
                    response.PurchaseOrder = po;
                    response.MergedOrderIdLists =
                        _purchaseOrderAsyncRepository.GetMergedPurchaseOrders(po.MergedWithPurchaseOrderId);

                    return Ok(response);
                }
                catch
                {
                    return NotFound("Incorrect ID of purchase order");
                }
            }
            await _emailSender.SendEmailAsync(message);
            return Ok();
        }
    }
}