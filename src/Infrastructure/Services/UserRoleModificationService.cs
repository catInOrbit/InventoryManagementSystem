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

        public async Task<IdentityResult> RoleDeletingHelper(string role)
        {
            IdentityResult result = null;
            await RemoveAllClaimHelper(role);
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (RoleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await RoleManager.RoleExistsAsync(role))
            {
                result = await RoleManager.DeleteAsync(new IdentityRole(role));
            }

            return result;
        }

        public async Task<IdentityResult> RemoveAllClaimHelper(string role)
        {
            IdentityResult result = new IdentityResult();
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (RoleManager == null)
                throw new Exception("roleManager null");

            var getRole = await RoleManager.FindByNameAsync(role);
            var roleClaims = await RoleManager.GetClaimsAsync(getRole);

            foreach (var roleClaim in roleClaims)
                result = await RoleManager.RemoveClaimAsync(getRole, roleClaim);
            return result;
        }

        public async Task<IdentityResult> ClaimCreatingHelper(string roleRequest, Claim authorizationOperation)
        {
            var getRole = await RoleManager.FindByNameAsync(roleRequest);
            IdentityResult result = new IdentityResult();

            //Done deleting

                //Create new role if role is new
            if (getRole == null)
            {
                var newRole = new IdentityRole(roleRequest);
                await RoleManager.CreateAsync(newRole);
                //Add claim to this role
                result = await RoleManager.AddClaimAsync(newRole, authorizationOperation);
            }

            else
            {
                //Add claim to this role (already exists)
                var getRoleAddNew = await RoleManager.FindByNameAsync(roleRequest);
                result = await RoleManager.AddClaimAsync(getRoleAddNew, authorizationOperation);
            }
           
            return result;
        }

    }
}