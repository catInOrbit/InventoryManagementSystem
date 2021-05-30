using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Identity.Models;
using Infrastructure.Services;
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
        private IUserAuthentication _userAuthentication;

        public AccountAuthorizationHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserAuthentication userAuthentication) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userAuthentication = userAuthentication;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            string page)
        {
            // var user = await _userManager.GetUserAsync(context.User);
            var user = await _userAuthentication.GetCurrentSessionUser();
            if(user == null)
                return Task.CompletedTask;
            var userRoles = await _userManager.GetRolesAsync(user);
            
            foreach (var role in userRoles)
            {
                if (role == "Manager")
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                else
                {
                    var roleGet = _roleManager.Roles.Single(x => x.Name == role);

                    var roleClaims = await _roleManager.GetClaimsAsync(roleGet); // A claim: exp: Page -- Action

                    var pageClaims = roleClaims.Where(x => x.Type == page); // List all claim for a page

                    if (!pageClaims.Any())
                    {
                        return Task.CompletedTask; //This user has unauthorized role for this page
                    }

                    //Check if user action is authorized

                    bool isActionAuthorized = false;
                    roleClaims.ToList().ForEach(e =>
                    {
                        if (e.Value.ToString() == requirement.Name)
                            isActionAuthorized = true;
                    });

                    if (isActionAuthorized)
                        context.Succeed(requirement);
                    else return Task.CompletedTask; //This user does not have permission for this page
                }
            }
            
            return Task.CompletedTask;
        }
    }
}