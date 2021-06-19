﻿using System;
using System.Collections.Generic;
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
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public async Task<string> GetRefreshTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
