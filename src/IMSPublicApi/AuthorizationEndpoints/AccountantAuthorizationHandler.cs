using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity.Models;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.PublicApi.AuthorizationEndpoints
{
    public class AccountantAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, UserInfo>
    {

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context, 
                OperationAuthorizationRequirement requirement,
                UserInfo resource)
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
