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

            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<Product>(m => m
                    .Ignore(p => p.BrandId)
                    .Ignore(p => p.CategoryId)
                    .Ignore(p => p.CreatedDate)
                    .Ignore(p => p.ModifiedDate)
                    .Ignore(p => p.ProductVariants)
                    .Ignore(p => p.CreatedBy)
                    .IdProperty(p => p.Id)
                );
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                index => index.Map<Product>(x => x.AutoMap())
            );
        }
    }
}