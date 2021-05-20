using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.PublicApi.Authorization
{
    public class AccountantAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, IMSUser>
    {
        UserManager<ApplicationUser> _userManager;

        public AccountantAuthorizationHandler(UserManager<ApplicationUser> 
            userManager)
        {
            _userManager = userManager;
        }
        
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                IMSUser resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != AuthenticationConstants.ApproveOperationName &&
                requirement.Name != AuthenticationConstants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(AuthenticationConstants.IMSAccountantRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
