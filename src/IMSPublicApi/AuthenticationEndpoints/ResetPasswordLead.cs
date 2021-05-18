using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class ResetPasswordLead : BaseAsyncEndpoint
        .WithRequest<ResetPasswordRequest>
        .WithResponse<ResetPasswordResponse>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly IEmailSender _emailSender;

        public ResetPasswordLead(ITokenClaimsService tokenClaimsService, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _tokenClaimsService = tokenClaimsService;
            _userManager = userManager;
            _emailSender = emailSender;
        }


        [HttpPost("api/resetpass")]
        [SwaggerOperation(
            Summary = "Authenticates a user",
            Description = "Authenticates a user",
            OperationId = "auth.authenticate",
            Tags = new[] { "IMSAuthenticationEndpoints" })
        ]
        public override async Task<ActionResult<ResetPasswordResponse>> HandleAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var response = new ResetPasswordResponse(request.CorrelationId());
            var user = await _userManager.FindByEmailAsync(request.Email);
            Console.WriteLine(user.Email);
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token =  await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // var callback = Url.Action(nameof(ResetPassword), "ResetPasswordLead", new { token, email = user.Email }, Request.Scheme);
                var callback = "test";
                var message = new EmailMessage(new string[] { user.Email }, "Reset password token", callback, null);
                Console.WriteLine(user.Email);
                await _emailSender.SendEmailAsync(message);
                response.Result = true;
            }

            return response;
        }
      
    }
}
