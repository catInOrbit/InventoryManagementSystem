using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
{
    public class PurchaseOrderSubmit : BaseAsyncEndpoint.WithRequest<PurchaseOrderSubmitRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;

        public PurchaseOrderSubmit(IEmailSender emailSender, IAuthorizationService authorizationService)
        {
            _emailSender = emailSender;
            _authorizationService = authorizationService;
        }
        
        public override async Task<ActionResult> HandleAsync(PurchaseOrderSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var subject = "Purchase Order " + DateTime.Now;

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PurchaseOrder",
                UserOperations.Update);

            if (!isAuthorized.Succeeded)
                return Unauthorized();
            
            try
            {
                var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
                var message = new EmailMessage(request.SupplierEmail, subject, "", files);
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