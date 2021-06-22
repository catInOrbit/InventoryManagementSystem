using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        JwtSecurityToken jwtToken;
        string userId = null;
        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserSession authentication, ITokenClaimsService tokenClaimsService, UserManager<ApplicationUser> userManager)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                    await attachUserToContext(context, userManager, authentication, tokenClaimsService,token);

            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, UserManager<ApplicationUser>  userManager, IUserSession userAuthentication, ITokenClaimsService tokenClaimsService, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
            try
            {
                ValidateToken(tokenHandler, token, key);
                var currentUser = await userManager.FindByIdAsync(userId);
                await userAuthentication.SaveUserAsync(currentUser, (await userManager.GetRolesAsync(currentUser))[0]);
            }
            catch
            {
                if (userAuthentication.GetCurrentSessionUser() != null)
                {
                    context.Request.Headers.Remove("Authorization");
                    
                    var tokenRefresh = await tokenClaimsService.GetRefreshTokenAsync((await userAuthentication.GetCurrentSessionUser()).Email);
                    if (tokenRefresh != null)
                    {
                        try
                        {
                            ValidateToken(tokenHandler, tokenRefresh, key);
                            context.Request.Headers.Add("Authorization",tokenRefresh);
                        }
                        catch (Exception e)
                        {
                            context.Items["User"] = null;
                        }
                    }
                }

                else
                    context.Items["User"] = null;
            }
        }

        private void ValidateToken(JwtSecurityTokenHandler tokenHandler, string token, byte[] key)
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            jwtToken = (JwtSecurityToken)validatedToken;
            userId = jwtToken.Claims.First(x => x.Type == "id").Value;
        }

    }
}