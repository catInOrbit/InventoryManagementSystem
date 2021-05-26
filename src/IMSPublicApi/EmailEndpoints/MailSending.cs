using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.EmailEndpoints
{

    public class MailSending : BaseAsyncEndpoint.WithRequest<MailSendingRequest>.WithResponse<MailSendingResponse>
    {
        private readonly IEmailSender _emailSender;

        public MailSending(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost("api/mailservice")]
        [SwaggerOperation(
            Summary = "Send email to specified email",
            Description = "Send email to specified email",
            OperationId = "resetpass",
            Tags = new[] { "ResetPasswordEndpoints" })
        ]
        public override async Task<ActionResult<MailSendingResponse>> HandleAsync(MailSendingRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new MailSendingResponse();
            
            var message = new EmailMessage(request.To, request.Content, request.Content, null);
            await _emailSender.SendEmailAsync(message);
            response.Result = true;
            return Ok(response);
        }
    }
}