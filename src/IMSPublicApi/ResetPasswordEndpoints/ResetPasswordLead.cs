using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.PublicApi.AuthenticationEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace InventoryManagementSystem.PublicApi.ResetPasswordEndpoints
{
    [Authorize]
    public class ResetPasswordLead : BaseAsyncEndpoint
        .WithRequest<ResetPasswordLeadRequest>
        .WithResponse<ResetPasswordLeadResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly IEmailSender _emailSender;

        public ResetPasswordLead(ITokenClaimsService tokenClaimsService, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _tokenClaimsService = tokenClaimsService;
            _userManager = userManager;
            _emailSender = emailSender;
        }


        [HttpPost("api/resetlead")]
        [SwaggerOperation(
            Summary = "Request a reset url sent to user's email",
            Description = "Request a reset url sent to user's email",
            OperationId = "resetpass",
            Tags = new[] { "ResetPasswordEndpoints" })
        ]
        public override async Task<ActionResult<ResetPasswordLeadResponse>> HandleAsync(ResetPasswordLeadRequest request, CancellationToken cancellationToken)
        {
            var response = new ResetPasswordLeadResponse(request.CorrelationId());
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token =  await _userManager.GeneratePasswordResetTokenAsync(user);
                var callback = token;
                var message = new EmailMessage(new string[] { user.Email }, "Reset password token", callback, null);
                Console.WriteLine(user.Email);
                Console.WriteLine(token);
                await _emailSender.SendEmailAsync(message);
                response.Result = true;
                response.Verbose = "Password reset sent to: " + user.Email;
            }

            else
            {
                Console.WriteLine("can not find user");
            }

            return response;
        }
    }
}
