using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public static class UserAuthorizationService
    {
        public static async Task<bool> Authorize(IAuthorizationService authorizationService, ClaimsPrincipal user, string pageAllowed,
            OperationAuthorizationRequirement operationAuthorizationRequirement)
        {
            var isAuthorized = await authorizationService.AuthorizeAsync(
                user, pageAllowed,
                operationAuthorizationRequirement);
            
            if (!isAuthorized.Succeeded)
                return false;
            return true;
        }
        
        public static async Task<bool> AuthorizeWithUserId(IAuthorizationService authorizationService, string userID, string idToMatch, ClaimsPrincipal user, string pageAllowed,
                   OperationAuthorizationRequirement operationAuthorizationRequirement)
       {
           var isAuthorized = await authorizationService.AuthorizeAsync(
               user, pageAllowed,
               operationAuthorizationRequirement);
           
           if (!isAuthorized.Succeeded && userID != idToMatch)
               return false;
           return true;
       } 
        
        
    }
}