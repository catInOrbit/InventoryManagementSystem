using System;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
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
                .DefaultMappingFor<ProductVariant>(m => m.IndexName(indexName));
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            client.Indices.Create(indexName,
                index => index.Map<ProductVariant>(x => x.AutoMap())
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
