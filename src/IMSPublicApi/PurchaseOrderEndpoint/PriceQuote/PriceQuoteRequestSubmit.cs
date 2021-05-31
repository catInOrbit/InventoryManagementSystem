using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PriceQuoteRequestSubmit : BaseAsyncEndpoint.WithRequest<PQSubmitRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<PriceQuoteOrder> _asyncRepository;
        private readonly IUserAuthentication _userAuthentication;

        
        public PriceQuoteRequestSubmit(IEmailSender emailSender, IAuthorizationService authorizationService, IAsyncRepository<PriceQuoteOrder> asyncRepository, IUserAuthentication userAuthentication)
        {
            _emailSender = emailSender;
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
        }
        
        [HttpPost("api/pricequote/submit")]
        [SwaggerOperation(
            Summary = "Submit price quote request",
            Description = "Submit price quote request",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromForm] PQSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
          
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                HttpContext.User, "PriceQuoteOrder",
                UserOperations.Update);

            if (!isAuthorized.Succeeded)
                return Unauthorized();

            var pqr = _asyncRepository.GetPriceQuoteByNumber(request.PriceQuoteNumberGet);
            pqr.PriceQuoteStatus = PriceQuoteType.Sent;
            pqr.ModifiedBy = (await _userAuthentication.GetCurrentSessionUser()).Id;
            pqr.ModifiedDate = DateTime.Now;
            await _asyncRepository.UpdateAsync(pqr);
            
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            var message = new EmailMessage(request.To, request.Subject, request.Content, files);
            await _emailSender.SendEmailAsync(message);
            return Ok();
        }
    }
}