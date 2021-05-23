using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services
{
    public class UserRoleModificationService
    {
        public UserManager<ApplicationUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public UserRoleModificationService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userUserManager)
        {
            RoleManager = roleManager;
            UserManager = userUserManager;
        }
        
        public UserRoleModificationService(RoleManager<IdentityRole> roleManager)
        {
            RoleManager = roleManager;
        }
        
        public UserRoleModificationService(UserManager<ApplicationUser> userUserManager)
        {
            UserManager = userUserManager;
        }

        public async Task<string> UserCreatingHelper(string password, string username, string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };
                
                await UserManager.CreateAsync(user, password);
            }

            else
            {
                user.Id = null;
            }
            //TODO: Throw Error saying authentication fail
            return user.Id;
        }

        public async Task<IdentityResult> RoleCreatingHelper(string uid, string role)
        {
            IdentityResult result = null;
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (RoleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await RoleManager.RoleExistsAsync(role))
            {
                result = await RoleManager.CreateAsync(new IdentityRole(role));
            }
            
            var user = await UserManager.FindByIdAsync(uid);

            if(user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }
            
            result = await UserManager.AddToRoleAsync(user, role);

            return result;
        }

        public async Task<IdentityResult> ClaimCreatingHelper(string role, Claim authorizationOperation)
        {
            var newRole = await RoleManager.FindByNameAsync(role);
            IdentityResult result = new IdentityResult();
            if (newRole == null)
            {
                newRole = new IdentityRole(role);
                await RoleManager.CreateAsync(newRole);
                result = await RoleManager.AddClaimAsync(newRole, authorizationOperation);
            }

            return result;
        }





    }
}