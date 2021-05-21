using System;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.PublicApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                        .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    // var catalogContext = services.GetRequiredService<CatalogContext>();
                    // await CatalogContextSeed.SeedAsync(catalogContext, loggerFactory);

                    //var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    // await AppIdentityDbContextSeed.SeedAsync(userManager, roleManager);

                    // SeedRole.Initialize(roleManager);
                    await SeedRole.Initialize(services, "test@12345Aha");
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
