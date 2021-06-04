using System;
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
            // AddDefaultMappings(defaultIndex, settings);

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
                .DefaultMappingFor<ProductSearchIndex>(m => m.IndexName(indexName));
        }

        private static void CreateIndex(IElasticClient client, string defaultIndexName)
        {
            client.Indices.Create(defaultIndexName,
                index => index.Map<ProductSearchIndex>(x => x.AutoMap())
            );
            
            client.Indices.Create("purchaseOrders",
                index => index.Map<PurchaseOrder>(x => x.AutoMap())
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
