using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Identity.Models;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.PublicApi.AuthorizationEndpoints
{
    public class AccountAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountAuthorizationHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            string page)
        {
            var user = await _userManager.GetUserAsync(context.User);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                var roleGet = _roleManager.Roles.Single(x => x.Name ==  role);
                var roleClaims = await _roleManager.GetClaimsAsync(roleGet); // A claim: exp: Create -- true, Update --false, Approve -- true
        
                var pageClaims = roleClaims.Where(x => x.Type == page ); // Page claim: exp: Product -- Create

                if (!pageClaims.Any())
                {
                    return Task.CompletedTask; //This user has unauthorized role for this page
                }

                List<Claim> duplicateClaimCheck = pageClaims.ToList().Intersect(roleClaims).ToList();
                
                if(duplicateClaimCheck.Count > 0)
                    context.Succeed(requirement);
                else return Task.CompletedTask; //This user does not have permission for this page
            }
            
            return Task.CompletedTask;
        }
    }
}