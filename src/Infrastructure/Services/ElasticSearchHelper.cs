using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using Nest;

namespace Infrastructure.Services
{
    public class ElasticSearchHelper<T> where T: BaseSearchIndex
    {
        private readonly string SearchQuery;
        private readonly string FieldToCheckExisting;
        private readonly string Index;

        private readonly IElasticClient _elasticClient;
        

        public ElasticSearchHelper(IElasticClient elasticClient, string searchQuery, string index)
        {
            _elasticClient = elasticClient;
            SearchQuery = searchQuery;
            Index = index;
        }
        
        public ElasticSearchHelper(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<ISearchResponse<T>> GetDocuments()
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
        
        public async Task<ISearchResponse<T>> GetDocumentsOldestFirst()
        {
            ISearchResponse<T> responseElastic;

            responseElastic = await _elasticClient.SearchAsync<T>
            (
                s => s.Sort(ss => ss.Ascending(p => p.LatestUpdateDate)).Size(2000).Index(Index).MatchAll());
                
            return responseElastic;
        }
        
        public async Task<ISearchResponse<ProductVariantSearchIndex>> CheckFieldExistProduct(string nameValue, string skuValue)
        {
            ISearchResponse<ProductVariantSearchIndex> responseElastic;
            // responseElastic = await _elasticClient.SearchAsync<ProductVariantSearchIndex>
            // (
            //     s => s.Size(2000).Index(Index).Query(q =>
            //         q.Bool(b =>
            //             b.Should(s =>
            //                     s.Term(t =>
            //                         t.Field(f => f.Name).Value(nameValue)),
            //                 s => s.Term(t =>
            //                     t.Field(f => f.Sku).Value(skuValue))
            //             ))));
            
            responseElastic = await _elasticClient.SearchAsync<ProductVariantSearchIndex>
            (
                s => s.Size(2000).Index(Index).Query(q =>
                    q.Bool(b =>
                        b.Should(
                            s =>
                                s.MultiMatch(t =>
                                    t.Query(nameValue).Type(TextQueryType.Phrase).Boost(10).Fields(f => 
                                        f.Field(f => f.Name).
                                          Field(f => f.Sku))
                        )))));
            
            return responseElastic;
        }
    }
}