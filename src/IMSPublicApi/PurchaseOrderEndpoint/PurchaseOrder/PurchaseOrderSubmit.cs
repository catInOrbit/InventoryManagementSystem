using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderSubmit : BaseAsyncEndpoint.WithRequest<PurchaseOrderSubmitRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;


        public PurchaseOrderSubmit(IEmailSender emailSender, IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository)
        {
            _emailSender = emailSender;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
        }
        
        [HttpPost("api/purchaseorder/submit")]
        [SwaggerOperation(
            Summary = "Submit",
            Description = "Submit new purchase order",
            OperationId = "catalog-items.create",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromForm] PurchaseOrderSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var subject = "Purchase Order " + DateTime.Now;

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Update);

            if (!isAuthorized.Succeeded)
                return Unauthorized();
            
            try
            {
                request.PurchaseOrder.PurchaseOrderStatus = PurchaseOrderStatusType.Sent;

                await _asyncRepository.UpdateAsync(request.PurchaseOrder);
                
                var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
                var message = new EmailMessage(request.To, request.Subject, request.Content, files);
                await _emailSender.SendEmailAsync(message);
                
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
    }
}