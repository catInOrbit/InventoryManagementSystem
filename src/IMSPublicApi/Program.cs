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
using InventoryManagementSystem.ApplicationCore.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;

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

                var elasticProductVariantRepos = services.GetRequiredService<IAsyncRepository<ProductVariant>>();
                var elasticProductRepos = services.GetRequiredService<IAsyncRepository<Product>>();

                var elasticPoRepos = services.GetRequiredService<IAsyncRepository<PurchaseOrder>>();
                var elasticRoRepos = services.GetRequiredService<IAsyncRepository<GoodsReceiptOrder>>();
                var elasticGiRepos = services.GetRequiredService<IAsyncRepository<GoodsIssueOrder>>();
                var elasticStRepos = services.GetRequiredService<IAsyncRepository<StockTakeOrder>>();
                var elasticSupRepos = services.GetRequiredService<IAsyncRepository<Supplier>>();
                var elasticPackageRepos = services.GetRequiredService<IAsyncRepository<Package>>();
                var elasticLocationRepos = services.GetRequiredService<IAsyncRepository<Location>>();
                var elasticCategoryRepos = services.GetRequiredService<IAsyncRepository<Category>>();

                var elasticClient = services.GetRequiredService<IElasticClient>();
                try
                {
                    ElasticIndexingService<BaseSearchIndex> els = new ElasticIndexingService<BaseSearchIndex>(elasticPoRepos, elasticRoRepos,
                        elasticStRepos, elasticGiRepos, elasticProductRepos,
                    elasticProductVariantRepos);

                    var productIndex = await els.IndexProduct(new PagingOption<ProductSearchIndex>(0, 0));
                    var productVariantIndex = await els.IndexProductVariant(new PagingOption<ProductVariantSearchIndex>(0, 0));
                    var poIndex =await els.IndexPurchasingOrder(new PagingOption<PurchaseOrderSearchIndex>(0, 0));
                    var roIndex = await els.IndexReceivingOrder(new PagingOption<GoodsReceiptOrderSearchIndex>(0, 0));
                    var giIndex = await els.IndexGoodsIssue(new PagingOption<GoodsIssueSearchIndex>(0, 0));
                    var stIndex = await els.IndexStockTake(new PagingOption<StockTakeSearchIndex>(0, 0));
                    
                    await new ElasticClientService<ProductSearchIndex>(services.GetRequiredService<ILogger<ElasticClientService<ProductSearchIndex>>>(), elasticClient).
                        ElasticSaveBulkAsync(productIndex.ResultList.ToArray(),    ElasticIndexConstant.PRODUCT_INDICES);
                    
                    await new ElasticClientService<ProductVariantSearchIndex>(services.GetRequiredService<ILogger<ElasticClientService<ProductVariantSearchIndex>>>(), elasticClient).
                        ElasticSaveBulkAsync(productVariantIndex.ResultList.ToArray(),    ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
                    
                    await new ElasticClientService<PurchaseOrderSearchIndex>(services.GetRequiredService<ILogger<ElasticClientService<PurchaseOrderSearchIndex>>>(), elasticClient).
                        ElasticSaveBulkAsync(poIndex.ResultList.ToArray(),    ElasticIndexConstant.PURCHASE_ORDERS);
                    
                    await new ElasticClientService<GoodsReceiptOrderSearchIndex>(services.GetRequiredService<ILogger<ElasticClientService<GoodsReceiptOrderSearchIndex>>>(), elasticClient).
                        ElasticSaveBulkAsync(roIndex.ResultList.ToArray(),    ElasticIndexConstant.RECEIVING_ORDERS);
                    
                    await new ElasticClientService<GoodsIssueSearchIndex>(services.GetRequiredService<ILogger<ElasticClientService<GoodsIssueSearchIndex>>>(), elasticClient).
                        ElasticSaveBulkAsync(giIndex.ResultList.ToArray(),    ElasticIndexConstant.GOODS_ISSUE_ORDERS);
                    
                    await new ElasticClientService<StockTakeSearchIndex>(services.GetRequiredService<ILogger<ElasticClientService<StockTakeSearchIndex>>>(), elasticClient).
                        ElasticSaveBulkAsync(stIndex.ResultList.ToArray(),    ElasticIndexConstant.STOCK_TAKE_ORDERS);
                    
                    await new ElasticClientService<Supplier>(services.GetRequiredService<ILogger<ElasticClientService<Supplier>>>(), elasticClient).
                        ElasticSaveBulkAsync((await elasticSupRepos.GetSuppliers(new PagingOption<Supplier>(0,0))).ResultList.ToArray(),    ElasticIndexConstant.SUPPLIERS);
                    await new ElasticClientService<Package>(services.GetRequiredService<ILogger<ElasticClientService<Package>>>(), elasticClient).
                        ElasticSaveBulkAsync((await elasticPackageRepos.GetPackages(new PagingOption<Package>(0,0))).ResultList.ToArray(), ElasticIndexConstant.PACKAGES);
                    await new ElasticClientService<Location>(services.GetRequiredService<ILogger<ElasticClientService<Location>>>(), elasticClient).
                        ElasticSaveBulkAsync((await elasticLocationRepos.GetLocation(new PagingOption<Location>(0,0))).ResultList.ToArray(), ElasticIndexConstant.LOCATIONS);
                    
                    await new ElasticClientService<Category>(services.GetRequiredService<ILogger<ElasticClientService<Category>>>(), elasticClient).ElasticSaveBulkAsync((await elasticCategoryRepos.GetCategory(new PagingOption<Category>(0,0))).ResultList.ToArray(), ElasticIndexConstant.CATEGORIES);

                    await SeedRole.Initialize(services);
                    
                    // BigQueryService bigQueryService = new BigQueryService();
                    // bigQueryService.Get3LinesData();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            
            await host.RunAsync();
           
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
