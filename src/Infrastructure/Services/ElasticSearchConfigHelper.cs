using System;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
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
            var password = configuration["elasticsearch:password"];
            var username = configuration["elasticsearch:username"];
            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex).BasicAuthentication("elastic", password).DisableDirectStreaming();;
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
            await client.Indices.DeleteAsync(ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
            await client.Indices.DeleteAsync(ElasticIndexConstant.PRODUCT_INDICES);

            await client.Indices.DeleteAsync(ElasticIndexConstant.PURCHASE_ORDERS);
            await client.Indices.DeleteAsync( ElasticIndexConstant.RECEIVING_ORDERS);
            await client.Indices.DeleteAsync(ElasticIndexConstant.STOCK_TAKE_ORDERS);
            await client.Indices.DeleteAsync( ElasticIndexConstant.GOODS_ISSUE_ORDERS);
            await client.Indices.DeleteAsync( ElasticIndexConstant.SUPPLIERS);
            await client.Indices.DeleteAsync( ElasticIndexConstant.PACKAGES);
            await client.Indices.DeleteAsync( ElasticIndexConstant.CATEGORIES);
            await client.Indices.DeleteAsync( ElasticIndexConstant.LOCATIONS);


            // await client.Indices.CreateAsync( ElasticIndexConstant.PRODUCT_VARIANT_INDICES,
            //     index 
            //         => index.Settings(s=> s.Analysis(
            //                 a => a.Tokenizers(
            //                     t => t.Pattern("hyphen_tokenizer", descriptor => descriptor.Pattern(".*-.*"))
            //                     )
            //                 ))
            //             .Map<ProductVariantSearchIndex>(x 
            //         => x.AutoMap().Properties(ps 
            //             => ps.Completion(c 
            //                 => c.Name(n => n.Suggest))))
            // );
            
            // await client.Indices.CreateAsync( ElasticIndexConstant.PRODUCT_VARIANT_INDICES,
            //     index 
            //         => index.Settings(s=> s.Analysis(
            //                 a => a.Tokenizers(
            //                     t => t.Pattern("hyphen_tokenizer", descriptor => descriptor.Pattern(".*-.*"))
            //                 )
            //             ))
            //             .Map<ProductVariantSearchIndex>(x 
            //                 => x.Properties(ps 
            //                     => ps.Text(t=>t.Name(n=>n.Name)).Completion(c 
            //                         => c.Name(n => n.Suggest))))
            // );
            
               
            await client.Indices.CreateAsync( ElasticIndexConstant.PRODUCT_VARIANT_INDICES,
                index 
                    => index.Settings(s=> s.Analysis(
                            a => a.Tokenizers(
                                t => t.Pattern("hyphen_tokenizer", descriptor => descriptor.Pattern(".*-.*"))
                            )
                        ))
                        .Map<ProductVariantSearchIndex>(x 
                            => x.AutoMap().Properties(ps 
                                => ps.Completion(c 
                                    => c.Name(n => n.Suggest))))
            );
            
        
            // await client.Indices.CreateAsync( ElasticIndexConstant.PRODUCT_INDICES,
            //     index 
            //         => index.Map<ProductSearchIndex>(x 
            //             => x.AutoMap().Properties(ps 
            //                 => ps.Nested<ProductVariantInfoInProductIndex>(ps => 
            //                     ps.Name(nn => nn.ProductVariantInfos).AutoMap()).Completion(c 
            //                     => c.Name(n => n.Suggest))))
            // );
            
            await client.Indices.CreateAsync( ElasticIndexConstant.PRODUCT_INDICES,
                index
                    => index.Settings(s => s.Analysis(a => a.Tokenizers(
                            t => t.Pattern("hyphen_tokenizer", descriptor => descriptor.Pattern(".*-.*"))
                        )))
                        .Map<ProductSearchIndex>(x 
                        => x.AutoMap().Properties(ps 
                            => ps.Nested<ProductVariantInfoInProductIndex>(ps => 
                                ps.Name(nn => nn.ProductVariantInfos).AutoMap()).Completion(c 
                                => c.Name(n => n.Suggest))))
            );
            await client.Indices.CreateAsync(ElasticIndexConstant.PURCHASE_ORDERS,
                index => index.Map<PurchaseOrderSearchIndex>(x 
                    => x.AutoMap().Properties(ps 
                        => ps.Completion(c 
                            => c.Name(n => n.Suggest))))
            );
            
            await client.Indices.CreateAsync( ElasticIndexConstant.RECEIVING_ORDERS,
                index => index.Map<GoodsReceiptOrderSearchIndex>(x => x.AutoMap())
            );
            
            await client.Indices.CreateAsync( ElasticIndexConstant.GOODS_ISSUE_ORDERS,
                index => index.Map<GoodsReceiptOrderSearchIndex>(x => x.AutoMap())
            );
            
            await client.Indices.CreateAsync( ElasticIndexConstant.STOCK_TAKE_ORDERS,
                index => index.Map<StockTakeSearchIndex>(x => x.AutoMap())
            );

            await client.Indices.CreateAsync( ElasticIndexConstant.SUPPLIERS,
                index => index.Map<Supplier>(x => x.AutoMap())
            );
            
            await client.Indices.CreateAsync( ElasticIndexConstant.CATEGORIES,
                index => index.Map<Category>(x => x.AutoMap())
            );
            
            await client.Indices.CreateAsync( ElasticIndexConstant.PACKAGES,
                index => index.Map<Package>(x => x.AutoMap())
            );

            await client.Indices.CreateAsync( ElasticIndexConstant.LOCATIONS,
                index => index.Map<Package>(x => x.AutoMap())
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
