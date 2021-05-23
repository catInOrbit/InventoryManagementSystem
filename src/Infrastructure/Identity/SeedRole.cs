using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity.DbContexts;
using Infrastructure.Identity.Models;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity
{
    public class SeedRole
    {

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
                user = new ApplicationUser
                {
                    UserName = "JakeA",
                    EmailConfirmed = true,
                    Email = "tmh1799@gmail.com",
                };
                await userManager.CreateAsync(user, testUserPw);
                
                
                var _userRepository = serviceProvider.GetRequiredService<IAsyncRepository<UserInfo>>();
                var newIMSUser = new UserInfo
                {
                    Id = user.Id,
                    Fullname =  "Huy Nguyen",
                    PhoneNumber =  "12345677",
                    Email = user.Email,
                    Username = user.UserName,
                    Address =  "aDDRESS",
                    IsActive =  true,
                    DateOfBirth = DateTime.Now
                };
                await _userRepository.AddAsync(newIMSUser, new CancellationToken());
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
                await roleManager.AddClaimAsync(identityRole, new Claim(AuthenticationConstants.CreateOperationName, true.ToString()));
                await roleManager.AddClaimAsync(identityRole, new Claim(AuthenticationConstants.UpdateOperationName, true.ToString()));
                await roleManager.AddClaimAsync(identityRole, new Claim(AuthenticationConstants.DeleteOperationName, true.ToString()));
                await roleManager.AddClaimAsync(identityRole, new Claim(AuthenticationConstants.ReadOperationName, true.ToString()));
                await roleManager.AddClaimAsync(identityRole, new Claim(AuthenticationConstants.ApproveOperationName, true.ToString()));
                await roleManager.AddClaimAsync(identityRole, new Claim(AuthenticationConstants.RejectOperationName, true.ToString()));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.CreateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.UpdateOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.DeleteOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.ApproveOperationName));
                await roleManager.AddClaimAsync(identityRole, new Claim("RolePermissionUpdate", AuthenticationConstants.RejectOperationName));
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