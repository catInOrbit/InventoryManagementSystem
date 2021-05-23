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
    public class AccountAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, UserInfo>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _page;

        public AccountAuthorizationHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, string page) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _page = page;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            UserInfo resource)
        {
            var user = _userManager.GetUserAsync(context.User);
            var userRoles = _userManager.GetRolesAsync(user.Result);
            foreach (var role in userRoles.Result)
            {
                IdentityRole roleGet = new IdentityRole(role);
                var roleClaims = _roleManager.GetClaimsAsync(roleGet); // A claim: exp: Create -- true, Update --false, Approve -- true

                var pageClaims = roleClaims.Result.Where(x => x.Type == _page ); // Page claim: exp: Product -- Create

                if (!pageClaims.Any())
                {
                    return Task.CompletedTask; //This user has unauthorized role for this page
                }

                List<Claim> duplicateClaimCheck = pageClaims.ToList().Intersect(roleClaims.Result).ToList();
                
                if(duplicateClaimCheck.Count > 0)
                    context.Succeed(requirement);
                else return Task.CompletedTask; //This user does not have permission for this page
            }
            
            return Task.CompletedTask;
        }
    }
}