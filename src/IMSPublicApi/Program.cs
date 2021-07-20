using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Hosting;
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

                var elasticProductVariantRepos = services.GetRequiredService<IAsyncRepository<ProductVariantSearchIndex>>();
                var elasticProductRepos = services.GetRequiredService<IAsyncRepository<ProductSearchIndex>>();

                var elasticPoRepos = services.GetRequiredService<IAsyncRepository<PurchaseOrderSearchIndex>>();
                var elasticRoRepos = services.GetRequiredService<IAsyncRepository<GoodsReceiptOrderSearchIndex>>();
                var elasticGiRepos = services.GetRequiredService<IAsyncRepository<GoodsIssueSearchIndex>>();
                var elasticStRepos = services.GetRequiredService<IAsyncRepository<StockTakeSearchIndex>>();
                var elasticSupRepos = services.GetRequiredService<IAsyncRepository<Supplier>>();
                var elasticPackageRepos = services.GetRequiredService<IAsyncRepository<Package>>();
                var elasticLocationRepos = services.GetRequiredService<IAsyncRepository<Location>>();

                try
                {
                    await elasticProductVariantRepos.ElasticSaveBulkAsync((await elasticProductVariantRepos.GetProductVariantForELIndexAsync(new PagingOption<ProductVariantSearchIndex>(0, 0))).ResultList.ToArray(),  ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
                    await elasticProductRepos.ElasticSaveBulkAsync((await elasticProductRepos.GetProductForELIndexAsync(new PagingOption<ProductSearchIndex>(0, 0))).ResultList.ToArray(),  ElasticIndexConstant.PRODUCT_INDICES);
                    await elasticPoRepos.ElasticSaveBulkAsync((await elasticPoRepos.GetPOForELIndexAsync(false,new PagingOption<PurchaseOrderSearchIndex>(0,0), new POSearchFilter())).ResultList.ToArray(),  ElasticIndexConstant.PURCHASE_ORDERS);
                    await elasticRoRepos.ElasticSaveBulkAsync((await elasticRoRepos.GetROForELIndexAsync(new PagingOption<GoodsReceiptOrderSearchIndex>(0,0),new ROSearchFilter())).ResultList.ToArray(), ElasticIndexConstant.RECEIVING_ORDERS);
                    await elasticGiRepos.ElasticSaveBulkAsync((await elasticGiRepos.GetGIForELIndexAsync(new PagingOption<GoodsIssueSearchIndex>(0,0), new GISearchFilter())).ResultList.ToArray(),  ElasticIndexConstant.GOODS_ISSUE_ORDERS);
                    await elasticStRepos.ElasticSaveBulkAsync((await elasticStRepos.GetSTForELIndexAsync(new PagingOption<StockTakeSearchIndex>(0,0), new STSearchFilter())).ResultList.ToArray(),  ElasticIndexConstant.STOCK_TAKE_ORDERS);
                    await elasticSupRepos.ElasticSaveBulkAsync((await elasticSupRepos.GetSuppliers(new PagingOption<Supplier>(0,0))).ResultList.ToArray(),    ElasticIndexConstant.SUPPLIERS);
                    await elasticPackageRepos.ElasticSaveBulkAsync((await elasticPackageRepos.GetPackages(new PagingOption<Package>(0,0))).ResultList.ToArray(), ElasticIndexConstant.PACKAGES);
                    await elasticLocationRepos.ElasticSaveBulkAsync((await elasticLocationRepos.ListAllAsync(new PagingOption<Location>(0,0))).ResultList.ToArray(), ElasticIndexConstant.LOCATIONS);
                    //
                    //
                    await SeedRole.Initialize(services, "test@12345Aha");
                    
                    // BigQueryService bigQueryService = new BigQueryService();
                    // bigQueryService.Get3LinesData();
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
