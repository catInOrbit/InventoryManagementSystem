using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
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
                var productRepos = services.GetRequiredService<IAsyncRepository<ProductVariant>>();

                var elasticProductRepos = services.GetRequiredService<IAsyncRepository<ProductSearchIndex>>();
                var elasticPoRepos = services.GetRequiredService<IAsyncRepository<PurchaseOrderSearchIndex>>();
                var elasticRoRepos = services.GetRequiredService<IAsyncRepository<GoodsReceiptOrderSearchIndex>>();
                var elasticGiRepos = services.GetRequiredService<IAsyncRepository<GoodsIssueSearchIndex>>();
                var elasticStRepos = services.GetRequiredService<IAsyncRepository<StockTakeSearchIndex>>();
                var elasticSupRepos = services.GetRequiredService<IAsyncRepository<Supplier>>();

                try
                {
                    
                    // await elasticProductRepos.ElasticSaveBulkAsync((await elasticProductRepos.GetProductForELIndexAsync(new PagingOption<ProductSearchIndex>(0, 0))).ResultList.ToArray(), "productindices");
                    // await elasticPoRepos.ElasticSaveBulkAsync((await elasticPoRepos.GetPOForELIndexAsync(new PagingOption<PurchaseOrderSearchIndex>(0,0), -99)).ResultList.ToArray(), "purchaseorders");
                    // await elasticRoRepos.ElasticSaveBulkAsync((await elasticRoRepos.GetROForELIndexAsync(new PagingOption<GoodsReceiptOrderSearchIndex>(0,0))).ResultList.ToArray(), "receivingorders");
                    // await elasticGiRepos.ElasticSaveBulkAsync((await elasticGiRepos.GetGIForELIndexAsync(new PagingOption<GoodsIssueSearchIndex>(0,0))).ResultList.ToArray(), "goodsissueorders");
                    // await elasticStRepos.ElasticSaveBulkAsync((await elasticStRepos.GetSTForELIndexAsync(new PagingOption<StockTakeSearchIndex>(0,0))).ResultList.ToArray(), "stocktakeorders");
                    // await elasticSupRepos.ElasticSaveBulkAsync((await elasticSupRepos.ListAllAsync(new PagingOption<Supplier>(0,0))).ResultList.ToArray(), "suppliers");
                        
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
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Listen(IPAddress.Any, Convert.ToInt32(Environment.GetEnvironmentVariable("PORT")));
                    }).UseStartup<Startup>();
                });
    }
}
