using System;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using Nest;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class ElasticClientService<T> : IElasticAsyncRepository<T> where T : BaseSearchIndex
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticClientService<T>> _logger;

        public ElasticClientService(ILogger<ElasticClientService<T>> logger, IElasticClient elasticClient)
        {
            _logger = logger;
            _elasticClient = elasticClient;
        }

        public async Task ElasticSaveSingleAsync(bool isSavingNew, T type, string index)
        {
            try
            {
                await ElasticDeleteSingleAsync(type, index);
                _logger.LogInformation("ElasticSaveSingleAsync: Type: " + type.GetType() + " || Reindexing");
                var response = await _elasticClient.IndexAsync(type, i => i.Index(index));
                if (!response.IsValid)
                    throw new Exception(response.DebugInformation);
            }
            catch
            {
                
            }
        }

        public async Task ElasticSaveManyAsync(T[] types)
        {
            // await _elasticClient.DeleteByQueryAsync<T>(q => q.MatchAll());
            var result = await _elasticClient.IndexManyAsync(types);
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    Console.WriteLine(itemWithError.Error);
                    throw new Exception();
                }
            }
        }
        
         public async Task ElasticSaveBulkAsync(T[] types, string index)
        {
            // await _elasticClient.DeleteByQueryAsync<T>(del => del
            //     .Query(q => q.QueryString(qs=>qs.Query("*")))
            // );

            // _elasticCache.AddRange(types);
            // _logger.LogInformation($"Elastic search cache count {_elasticCache.Count}");
            // _logger.LogInformation($"Elastic search cache type {_elasticCache.GetType()}");
            Console.WriteLine("Indexing " + types.Length + "objects of type " + types.GetType() + "| Index: " +index);
            var result = await _elasticClient.BulkAsync(b => b.Index(index).IndexMany(types));
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    throw new Exception();
                }
            }
        }
                
        public async Task ElasticDeleteSingleAsync(T type, string index)
        {
            var response = await _elasticClient.DeleteAsync<T>(type, u => u.Index(index));
            // Console.WriteLine("ElasticDeleteSingleAsync: Type: " + type.GetType() + " || Delete");
            _logger.LogInformation("ElasticDeleteSingleAsync: Type: " + type.GetType() + " || Delete");

            if (!response.IsValid)
            {
                _logger.LogWarning("ElasticDeleteSingleAsync: Problem: " + response.DebugInformation);
                // throw new Exception(response.Result.ToString());
            }
        }

    }
}