using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        JwtSecurityToken _jwtToken;
        private string _userId = null;
        private string _expiresTime = null;

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
            ApplicationUser currentUser = null;
            try
            {
                Console.WriteLine("Validating token");
                ValidateToken(tokenHandler, token, key);

                Console.WriteLine("Validating successful");

                currentUser = await userManager.FindByIdAsync(_userId);
                await userAuthentication.SaveUserAsync(currentUser, (await userManager.GetRolesAsync(currentUser))[0]);

                var newToken = await tokenClaimsService.GenerateRefreshTokenAsync((await userAuthentication.GetCurrentSessionUser()).Email);

                await tokenClaimsService.SaveRefreshTokenForUser(currentUser, newToken);
                Console.WriteLine("New Token Refresh saved: " + newToken);

            }
            catch
            {
                Console.WriteLine("Fail Token Validation");

                currentUser = await userManager.FindByIdAsync(_userId);
                if (currentUser != null)
                {
                    // context.Request.Headers.Remove("Authorization");
                    
                    // var tokenRefresh = await tokenClaimsService.GenerateRefreshTokenAsync((await userAuthentication.GetCurrentSessionUser()).Email);
                    var tokenRefresh = await tokenClaimsService.GetRefreshTokenAsync((currentUser));

                    if (tokenRefresh != null)
                    {
                        Console.WriteLine("Successfully got token refresh for user in database");

                        try
                        {
                            ValidateToken(tokenHandler, tokenRefresh, key);
                            Console.WriteLine("Successfully validated user token for user in database");

                            currentUser = await userManager.FindByIdAsync(_userId);
                            var newToken = await tokenClaimsService.GenerateRefreshTokenAsync((currentUser).Email);

                            await tokenClaimsService.SaveRefreshTokenForUser(currentUser, newToken);
                            Console.WriteLine("New Token Refresh saved: " + newToken);

                            // context.Request.Headers.Add("Authorization",newToken);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Fail validation of refresh token");

                            await userAuthentication.RemoveUserAsync(currentUser);
                            context.Items["User"] = null;
                        }
                    }
                }

                else
                {
                    Console.WriteLine("Fail validation of token");

                    await userAuthentication.RemoveUserAsync(currentUser);
                    context.Items["User"] = null;
                }
            }
        }

        private void ValidateToken(JwtSecurityTokenHandler tokenHandler, string token, byte[] key)
        {

            var tokenRead = tokenHandler.ReadToken(token);
            var tokenJWT = tokenRead as JwtSecurityToken;
            if (tokenJWT != null) _userId = tokenJWT.Claims.FirstOrDefault(x => x.Type == "id").Value.ToString();
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            _jwtToken = (JwtSecurityToken)validatedToken;
            _userId = _jwtToken.Claims.First(x => x.Type == "id").Value;
            _expiresTime = _jwtToken.Claims.First(x => x.Type == "exp").Value;
            Console.WriteLine("User ID got from JWT: " + _userId);
            Console.WriteLine("_expiresTime timespan " + _expiresTime);
        }

    }
}