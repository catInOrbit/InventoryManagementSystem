using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.ApplicationCore.Services
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
        
        public async Task<bool> CheckRoleNameExistsHelper(string roleName)
        {
            IdentityResult result = null;
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (RoleManager == null)
            {
                throw new Exception("roleManager null");
            }

            foreach (var role in RoleManager.Roles)
            {
                if (role.Name == roleName)
                    return true;
            }
            return false;
            }
        
        public async Task<IdentityResult> RoleUpdatingHelper(string userID, string role)
        {
            IdentityResult result = new IdentityResult();
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (RoleManager == null)
            {
                throw new Exception("roleManager null");
            }

            var user = await UserManager.FindByIdAsync(userID);

            if (await RoleManager.RoleExistsAsync(role) && user != null)
            {
                result = await UserManager.AddToRoleAsync(user, role);
            }

            return result;
        }

        public async Task<IdentityResult> RoleDeletingHelper(IdentityRole role)
        {
            IdentityResult result = null;
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (RoleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (await RoleManager.RoleExistsAsync(role.Name))
            {
                await RemoveAllClaimHelper(role);
                var allRoles =  RoleManager.Roles.ToList();

                foreach (var roleToDelete in allRoles)
                {
                    if (roleToDelete.Name == role.Name)
                    {
                        result = await RoleManager.DeleteAsync(roleToDelete);
                    }
                }
            }

            return result;
        }

        public async Task<IdentityResult> RemoveAllClaimHelper(IdentityRole role)
        {
            IdentityResult result = new IdentityResult();
            // var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (RoleManager == null)
                throw new Exception("roleManager null");

            var getRole = role;
            var roleClaims = await RoleManager.GetClaimsAsync(getRole);

            foreach (var roleClaim in roleClaims)
                result = await RoleManager.RemoveClaimAsync(getRole, roleClaim);
            return result;
        }

        public async Task<IdentityResult> ClaimCreatingHelper(string roleName, Claim authorizationOperation)
        {
            var getRole = await RoleManager.FindByNameAsync(roleName);
            IdentityResult result = new IdentityResult();

            //Done deleting

                //Create new role if role is new
            if (getRole == null)
            {
                var newRole = new IdentityRole(roleName);
                await RoleManager.CreateAsync(newRole);
                //Add claim to this role
                result = await RoleManager.AddClaimAsync(newRole, authorizationOperation);
            }

            else
            {
                //Add claim to this role (already exists)
                var getRoleAddNew = await RoleManager.FindByNameAsync(roleName);
                result = await RoleManager.AddClaimAsync(getRoleAddNew, authorizationOperation);
            }
           
            return result;
        }

        public async Task<IList<Claim>> ClaimGettingHelper()
        {
            var claims = await RoleManager.GetClaimsAsync(RoleManager.Roles.FirstOrDefault());
            
            return claims;
        }
        
        public async Task<IList<Claim>> GetClaimOfSpecificRole(IdentityRole role)
        {
            var claims = await RoleManager.GetClaimsAsync(role);
            return claims;
        }

    }
}