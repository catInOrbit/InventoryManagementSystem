using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using Nest;

namespace Infrastructure.Services
{
    public class ElasticSearchHelper<T> where T: BaseSearchIndex
    {
        private readonly string SearchQuery;
        private readonly string Index;

        private readonly IElasticClient _elasticClient;
        

        public ElasticSearchHelper(IElasticClient elasticClient, string searchQuery, string index)
        {
            _elasticClient = elasticClient;
            SearchQuery = searchQuery;
            Index = index;
        }

        public async Task<ISearchResponse<T>> SearchDocuments()
        {
            ISearchResponse<T> responseElastic;

            if (SearchQuery == null)
            {
                responseElastic = await _elasticClient.SearchAsync<T>
                (
                    s => s.Sort(ss => ss.Descending(p => p.LatestUpdateDate)).Size(2000).Index(Index).MatchAll());
                    
                return responseElastic;
            }

            else
            {
                responseElastic = await _elasticClient.SearchAsync<T>
                (
                    s => s.Size(2000).Index(Index).
                        Query(q =>q.
                            QueryString(d =>d.Query('*' + SearchQuery + '*'))));

                return responseElastic;
            }
        }
    }
}