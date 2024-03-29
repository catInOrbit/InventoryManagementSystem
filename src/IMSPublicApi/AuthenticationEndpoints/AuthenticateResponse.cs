﻿using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.AuthenticationEndpoints
{
    public class AuthenticateResponse : BaseResponse
    {
        public AuthenticateResponse(Guid correlationId) : base(correlationId)
        {
        }

        public AuthenticateResponse()
        {
        }
        public bool Result { get; set; } = false;
        public string Verbose { get; set; }

        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsLockedOut { get; set; } = false;
        public bool IsNotAllowed { get; set; } = false;
        public bool RequiresTwoFactor { get; set; } = false;
        public string UserRole { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<string> PageAuthorized { get; set; } = new List<string>();
        
        public CompanyInfo CompanyInfo { get; set; }
    }

}
