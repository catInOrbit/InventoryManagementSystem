using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class IdentityTokenClaimService : ITokenClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public IdentityTokenClaimService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(1),
                // Expires = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).ToLocalTime().AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
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
                Expires = DateTime.UtcNow.AddHours(1),
                // Expires = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).ToLocalTime().AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
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
