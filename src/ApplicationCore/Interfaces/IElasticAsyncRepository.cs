using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using Nest;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IElasticAsyncRepository<T> where T : BaseEntity
    {
        Task<ISearchResponse<T>> GetAllWithQuery(string index, string query);
    }

    public class ElasticSearchImplementation<T> : IElasticAsyncRepository<T> where T : BaseEntity
    {
        private IElasticClient _elasticClient;

        public ElasticSearchImplementation(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<ISearchResponse<T>> GetAllWithQuery(string index, string query)
        {
            var responseElastic = await _elasticClient.SearchAsync<T>
            (
                s => s.Size(2000).Index(index).Query(q => q.QueryString(d => d.Query('*' + query + '*'))));

            return responseElastic;
        }
    }
}