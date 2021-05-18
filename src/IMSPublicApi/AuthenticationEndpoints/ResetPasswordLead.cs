using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class ResetPasswordLead : BaseAsyncEndpoint
        .WithRequest<ResetPasswordRequest>
        .WithResponse<ResetPasswordResponse>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenClaimsService _tokenClaimsService;

        public ResetPasswordLead(ITokenClaimsService tokenClaimsService, UserManager<IdentityUser> userManager)
        {
            _tokenClaimsService = tokenClaimsService;
            _userManager = userManager;
        }


        public override async Task<ActionResult<ResetPasswordResponse>> HandleAsync(ResetPasswordRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new ResetPasswordResponse(request.CorrelationId());
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token =  await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
                response.Result = result.Succeeded;
            }

            return response;
        }
    }
}
