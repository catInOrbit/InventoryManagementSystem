using System;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Infrastructure.Services
{
    public static class ElasticSearchConfigHelper
    {
        public static void AddElasticsearch(
            this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];
            var priceQuoteIndex = configuration["elasticsearch:priceQuoteIndex"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex).BasicAuthentication("elastic", "LDBFEOTonZjDL1jueHMlKXcC");;
            AddDefaultMappings(defaultIndex, settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
            
            try
            {
                CreateIndex(client, defaultIndex);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddDefaultMappings(string indexName, ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<PurchaseOrderSearchIndex>(m => m.IndexName(indexName));
            
        }

        private static async Task CreateIndex(IElasticClient client, string defaultIndexName)
        {
            await client.Indices.DeleteAsync("productindices");
            await client.Indices.DeleteAsync("purchaseorders");
            await client.Indices.DeleteAsync("receivingorders");
            await client.Indices.DeleteAsync("goodsissueorders");
            await client.Indices.DeleteAsync("supplier");

            await client.Indices.CreateAsync("productindices",
                index 
                    => index.Map<ProductSearchIndex>(x 
                    => x.AutoMap().Properties(ps 
                        => ps.Completion(c 
                            => c.Name(n => n.Suggest))))
            );
            
            await client.Indices.CreateAsync("purchaseorders",
                index => index.Map<PurchaseOrderSearchIndex>(x 
                    => x.AutoMap().Properties(ps 
                        => ps.Completion(c 
                            => c.Name(n => n.Suggest))))
            );
            
            await client.Indices.CreateAsync("receivingorders",
                index => index.Map<GoodsReceiptOrderSearchIndex>(x => x.AutoMap())
            );
            
            await client.Indices.CreateAsync("goodsissueorders",
                index => index.Map<GoodsReceiptOrderSearchIndex>(x => x.AutoMap())
            );
            
            await client.Indices.CreateAsync("stocktakeorders",
                index => index.Map<StockTakeSearchIndex>(x => x.AutoMap())
            );

            await client.Indices.CreateAsync("suppliers",
                index => index.Map<Supplier>(x => x.AutoMap())
            );

            // client.Indices.Create(indexName, i => i
            //     .Settings(s => s
            //         .NumberOfShards(2)
            //         .NumberOfReplicas(0)
            //         .Setting("index.mapping.nested_objects.limit", 12000)
            //     )
            //     .Map<ProductVariant>(map => map
            //         .AutoMap()
            //         .Properties(ps => ps
            //             .Nested<VariantValue>(n => n
            //                 .Name(p => p.Name)
            //                 .AutoMap()
            //             )
            //             .Nested<ProductSerialNumber>(n => n
            //                 .Name(p => p.Name)
            //                 .AutoMap()
            //             )
            //             // .Object<Product>(n => n.Name(p => p.Name).AutoMap())
            //         )
            //     )
            // );
        }
    }
}
