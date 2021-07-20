using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity
{
    public class SeedRole
    {
        private const string GETUSERSPAGE = "GetUsers";
        private const string REGISTRATION = "Registration";

        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new IdentityAndProductDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<IdentityAndProductDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, testUserPw);
                await EnsureRole(serviceProvider, managerID, "Manager");
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                   string testUserPw)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByNameAsync("JakeA");
            if (user == null)
            {
                var dob = DateTime.UtcNow;
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Huy Nguyen",
                    PhoneNumber = "12345677",
                    Email = "tmh1799@gmail.com",
                    UserName = "JakeA",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
               var result = await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }


            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                var identityRole = new IdentityRole(role);

                IR = await roleManager.CreateAsync(identityRole);
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.CreateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.UpdateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.DeleteOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.ApproveOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.RejectOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(REGISTRATION, AuthenticationConstants.CreateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.CreateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.UpdateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.DeleteOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.ApproveOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.RejectOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.UpdateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.DeleteOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.ApproveOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim(GETUSERSPAGE, AuthenticationConstants.RejectOperationName));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

          

            return IR;
        }
        
        public async Task<IdentityResult> ClaimCreatingHelper(IServiceProvider serviceProvider, string role, Claim authorizationOperation)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var newRole = await roleManager.FindByNameAsync(role);
            IdentityResult result = new IdentityResult();
            if (newRole == null)
            {
                newRole = new IdentityRole(role);
                await roleManager.CreateAsync(newRole);
                result = await roleManager.AddClaimAsync(newRole, authorizationOperation);
            }

            return result;
        }
    }
}