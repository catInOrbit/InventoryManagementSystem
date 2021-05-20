using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
namespace Infrastructure.Identity
{
    public class SeedRole
    {
        public static async Task Initialize(RoleManager<IdentityRole> roleManager)
        {
            var accountManagerRole = await roleManager.FindByNameAsync("Manager");

            if (accountManagerRole == null)
            {
                accountManagerRole = new IdentityRole("Account Manager");
                await roleManager.CreateAsync(accountManagerRole);

                await roleManager.AddClaimAsync(accountManagerRole, new Claim(AuthenticationConstants.CreateOperationName, "true"));
                await roleManager.AddClaimAsync(accountManagerRole, new Claim(AuthenticationConstants.ReadOperationName, "true"));
                await roleManager.AddClaimAsync(accountManagerRole, new Claim(AuthenticationConstants.UpdateOperationName, "true"));
                await roleManager.AddClaimAsync(accountManagerRole, new Claim(AuthenticationConstants.DeleteOperationName, "true"));
                await roleManager.AddClaimAsync(accountManagerRole, new Claim(AuthenticationConstants.ApproveOperationName, "true"));
                await roleManager.AddClaimAsync(accountManagerRole, new Claim(AuthenticationConstants.RejectOperationName, "true"));
            }
        }
    }
}