using System.Threading.Tasks;
using Infrastructure.Data;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace InventoryManagementSystem.PublicApi.AuthorizationEndpoints
{
    public class ManagerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, UserInfo>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, 
            UserInfo resource)
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
