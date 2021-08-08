using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore.Constants;
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

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new IdentityAndProductDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<IdentityAndProductDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                // allowed user can create and edit contacts that they create
                await EnsureUsers(serviceProvider);
            }
        }

        private static async Task EnsureUsers(IServiceProvider serviceProvider)
        {
            
            await EnsureRoleCreation(serviceProvider);

            var userManagerService = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var userManager = await userManagerService.FindByNameAsync("tmh1799@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Huy Nguyen",
                    PhoneNumber = "12345677",
                    Email = "tmh1799@gmail.com",
                    UserName = "tmh1799@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
               var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }

            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.MANAGER);
            
            userManager = await userManagerService.FindByNameAsync("manager01@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Manager Name 1",
                    PhoneNumber = "12345677",
                    Email = "manager01@gmail.com",
                    UserName = "manager01@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.MANAGER);
            
            userManager = await userManagerService.FindByNameAsync("manager02gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Manager Name 2",
                    PhoneNumber = "12345677",
                    Email = "manager02gmail.com",
                    UserName = "manager02gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.MANAGER);
            
            userManager = await userManagerService.FindByNameAsync("saleman01@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Saleman Name",
                    PhoneNumber = "12345677",
                    Email = "saleman01@gmail.com",
                    UserName = "saleman01@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.SALEMAN);
            
            userManager = await userManagerService.FindByNameAsync("saleman02@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Saleman Name",
                    PhoneNumber = "12345677",
                    Email = "saleman02@gmail.com",
                    UserName = "saleman02@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.SALEMAN);
            
            userManager = await userManagerService.FindByNameAsync("saleman03@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Saleman Name",
                    PhoneNumber = "12345677",
                    Email = "saleman03@gmail.com",
                    UserName = "saleman03@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.SALEMAN);
            
            userManager = await userManagerService.FindByNameAsync("accountant01@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Accountant Name",
                    PhoneNumber = "12345677",
                    Email = "accountant01@gmail.com",
                    UserName = "accountant01@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.ACCOUNTANT);
            
            userManager = await userManagerService.FindByNameAsync("accountant02@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Accountant Name",
                    PhoneNumber = "12345677",
                    Email = "accountant02@gmail.com",
                    UserName = "accountant02@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.ACCOUNTANT);
            
            userManager = await userManagerService.FindByNameAsync("accountant03@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "Accountant Name",
                    PhoneNumber = "12345677",
                    Email = "accountant03@gmail.com",
                    UserName = "accountant03@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.ACCOUNTANT);

            
            userManager = await userManagerService.FindByNameAsync("stockkeeper01@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "StockKeeper Name",
                    PhoneNumber = "12345677",
                    Email = "stockkeeper01@gmail.com",
                    UserName = "stockkeeper01@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.STOCKKEEPER);
            
            
            userManager = await userManagerService.FindByNameAsync("stockkeeper02@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "StockKeeper Name",
                    PhoneNumber = "12345677",
                    Email = "stockkeeper02@gmail.com",
                    UserName = "stockkeeper02@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.STOCKKEEPER);
            
            userManager = await userManagerService.FindByNameAsync("stockkeeper03@gmail.com");
            if (userManager == null)
            {
                var dob = DateTime.UtcNow;
                userManager = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = "StockKeeper Name",
                    PhoneNumber = "12345677",
                    Email = "stockkeeper03@gmail.com",
                    UserName = "stockkeeper03@gmail.com",
                    Address = "aDDRESS",
                    IsActive = true,
                    DateOfBirth = dob,
                    DateOfBirthNormalizedString = dob.ToString("yyyy-MM-dd")
                };
                var result = await userManagerService.CreateAsync(userManager, "test@12345Aha");
            }
            
            await EnsureUserRoleAssign(serviceProvider, userManager.Id, AuthorizedRoleConstants.STOCKKEEPER);
        }

        private static async Task<IdentityResult> EnsureUserRoleAssign(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
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
        
         private static async Task<IdentityResult> EnsureRoleCreation(IServiceProvider serviceProvider)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityRole identityRole;
            
            if (!await roleManager.RoleExistsAsync(AuthorizedRoleConstants.MANAGER))
            {
                identityRole = new IdentityRole
                {
                    Id = "IMS_MN",
                    Name = AuthorizedRoleConstants.MANAGER,
                    NormalizedName = AuthorizedRoleConstants.MANAGER.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                
                IR = await roleManager.CreateAsync(identityRole);
            }

            if (!await roleManager.RoleExistsAsync(AuthorizedRoleConstants.SALEMAN))
            {
                identityRole = new IdentityRole
                {
                    Id = "IMS_SM",
                    Name = AuthorizedRoleConstants.SALEMAN,
                    NormalizedName = AuthorizedRoleConstants.SALEMAN.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                
                IR = await roleManager.CreateAsync(identityRole);
                
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.REQUISITION, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PRODUCT_SEARCH, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PURCHASEORDER_SEARCH, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.SUPPLIER_SEARCH, ""));
            }
            
            if (!await roleManager.RoleExistsAsync(AuthorizedRoleConstants.ACCOUNTANT))
            {
                identityRole = new IdentityRole
                {
                    Id = "IMS_AC",
                    Name = AuthorizedRoleConstants.ACCOUNTANT,
                    NormalizedName = AuthorizedRoleConstants.ACCOUNTANT.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                
                IR = await roleManager.CreateAsync(identityRole);
                
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PURCHASEORDER, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PRICEQUOTEORDER, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.GOODSRECEIPT, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.GOODSISSUE, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.STOCKTAKEORDER, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PRODUCT_SEARCH, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PACKAGE_SEARCH, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.SUPPLIER_SEARCH, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PURCHASEORDER_SEARCH, ""));
            }
            
            if (!await roleManager.RoleExistsAsync(AuthorizedRoleConstants.STOCKKEEPER))
            {
                identityRole = new IdentityRole
                {
                    Id = "IMS_SK",
                    Name = AuthorizedRoleConstants.STOCKKEEPER,
                    NormalizedName = AuthorizedRoleConstants.STOCKKEEPER.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                
                IR = await roleManager.CreateAsync(identityRole);
                
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.GOODSRECEIPT, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.GOODSISSUE, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PRODUCT_SEARCH, ""));
                await roleManager.AddClaimAsync(identityRole, new Claim(PageConstant.PACKAGE_SEARCH, ""));
                
            }
          
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