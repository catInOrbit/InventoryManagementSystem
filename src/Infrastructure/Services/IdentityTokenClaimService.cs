using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ILogger = Castle.Core.Logging.ILogger;

namespace Infrastructure.Services
{
    public class IdentityTokenClaimService : ITokenClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ILogger<IdentityTokenClaimService> _logger;
        
        public IdentityTokenClaimService(UserManager<ApplicationUser> userManager, ILogger<IdentityTokenClaimService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                // Expires = DateTime.UtcNow.AddSeconds(30),
                Expires = DateTime.Now.AddHours(1),

                // Expires = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now).ToLocalTime().AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogInformation("Current server time is: " + DateTime.Now);
            _logger.LogInformation("Token login generated and expires at: " + tokenDescriptor.Expires);
            return tokenHandler.WriteToken(token);
        }
        
        public async Task<string> GenerateRefreshTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                // Expires = DateTime.UtcNow.AddHours(1),
                Expires = DateTime.Now.AddHours(1),

                // Expires = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now).ToLocalTime().AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogInformation("Current server time is: " + DateTime.Now);
            _logger.LogInformation("Token refresh generated and expires at: " + tokenDescriptor.Expires);
            return  tokenHandler.WriteToken(token);
        }

        public async Task<string> GetRefreshTokenAsync(ApplicationUser user)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken");
        }

        public async Task SaveRefreshTokenForUser(ApplicationUser user, string tokenRefresh)
        {
            if (await _userManager.GetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken") != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken");
                await _userManager.SetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken", tokenRefresh);
            }
            else
                await _userManager.SetAuthenticationTokenAsync(user, "IMSPublicAPI", "RefreshToken", tokenRefresh);
        
        }
    }
}
