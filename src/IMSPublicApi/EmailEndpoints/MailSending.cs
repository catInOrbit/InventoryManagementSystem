using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Castle.Core.Internal;
using Infrastructure;
using InventoryManagementSystem.ApplicationCore;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.EmailEndpoints
{

    public class SendMail : BaseAsyncEndpoint.WithRequest<MailSendingRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;
        private readonly IAsyncRepository<PurchaseOrder> _purchaseOrderAsyncRepository;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;
        private readonly IUserSession _userSession;
        public SendMail(IEmailSender emailSender, IAsyncRepository<PurchaseOrder> purchaseOrderAsyncRepository, IElasticAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, IUserSession userSession)
        {
            _emailSender = emailSender;
            _purchaseOrderAsyncRepository = purchaseOrderAsyncRepository;
            _indexAsyncRepository = indexAsyncRepository;
            _userSession = userSession;
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
                        _purchaseOrderAsyncRepository.GetMergedPurchaseOrders(po.Id);
                    
                    foreach (var orderItem in po.PurchaseOrderProduct)
                        orderItem.IsShowingProductVariant = true;
                    var currentSessionUser = await _userSession.GetCurrentSessionUser();

                    po.Transaction = TransactionUpdateHelper.UpdateMailTransaction(po.Transaction,
                        UserTransactionActionType.Modify,
                        TransactionType.Purchase, currentSessionUser.Id, po.Id, String.Join(",", request.To));
                    
                    await _emailSender.SendEmailAsync(message);
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