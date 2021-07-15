using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.EmailEndpoints
{

    public class MailSending : BaseAsyncEndpoint.WithRequest<MailSendingRequest>.WithoutResponse
    {
        private readonly IEmailSender _emailSender;

        public MailSending(IEmailSender emailSender)
        {
            _emailSender = emailSender;
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
            await _emailSender.SendEmailAsync(message);
            return Ok();
        }
    }
}