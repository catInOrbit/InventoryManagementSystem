using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.PublicApi.Authorization
{
    public class ManagerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, IMSUser>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, 
            IMSUser resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            
            // Administrators can do anything.
            if (context.User.IsInRole(AuthenticationConstants.IMSManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
