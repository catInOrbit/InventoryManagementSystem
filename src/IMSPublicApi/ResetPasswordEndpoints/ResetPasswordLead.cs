using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ResetPasswordEndpoints
{
    public class ResetPasswordLead : BaseAsyncEndpoint.WithRequest<ResetPasswordLeadRequest>.WithResponse<ResetPasswordLeadResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly IEmailSender _emailSender;
        private IUserSession _userAuthentication;

        public ResetPasswordLead(ITokenClaimsService tokenClaimsService, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IUserSession userAuthentication)
        {
            _tokenClaimsService = tokenClaimsService;
            _userManager = userManager;
            _emailSender = emailSender;
            _userAuthentication = userAuthentication;
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
            if (user != null)
            {
                var token =  await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordGen = "NEW#" + Guid.NewGuid().ToString().Substring(0, 12);
                var message = new EmailMessage(new string[] { user.Email }, "New password: ", passwordGen, null);
                var result = await _userManager.ResetPasswordAsync(user, token, passwordGen);
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
