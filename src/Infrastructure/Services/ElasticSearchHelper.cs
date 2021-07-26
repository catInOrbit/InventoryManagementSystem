using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
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
        
        public async Task<ISearchResponse<ProductVariantSearchIndex>> CheckFieldExistProductVariant(string value)
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
                s => s.Size(2000).Index(ElasticIndexConstant.PRODUCT_VARIANT_INDICES).Query(q =>
                    q.Bool(b =>
                        b.Should(
                            s =>
                                s.MultiMatch(t =>
                                    t.Query(value).Type(TextQueryType.Phrase).Boost(10).Fields(f => 
                                        f.Field(f => f.Name).
                                          Field(f => f.Sku).
                                          Field(f => f.Barcode))
                        )))));
            
            return responseElastic;
        }
        
        public async Task<ISearchResponse<ProductSearchIndex>> CheckFieldExistProduct(string value)
        {
            ISearchResponse<ProductSearchIndex> responseElastic;
            
            responseElastic = await _elasticClient.SearchAsync<ProductSearchIndex>
            (
                s => s.Size(2000).Index(ElasticIndexConstant.PRODUCT_INDICES).Query(q =>
                    q.Bool(b =>
                        b.Should(
                            s =>
                                s.MultiMatch(t =>
                                    t.Query(value).Type(TextQueryType.Phrase).Boost(10).Fields(f => 
                                        f.Field(f => f.Name)
                                ))))));
            return responseElastic;
        }
        
        public async Task<ISearchResponse<Location>> CheckFieldExistLocation(string value)
        {
            ISearchResponse<Location> responseElastic;
            
            responseElastic = await _elasticClient.SearchAsync<Location>
            (
                s => s.Size(2000).Index(ElasticIndexConstant.LOCATIONS).Query(q =>
                    q.Bool(b =>
                        b.Should(
                            s =>
                                s.MultiMatch(t =>
                                    t.Query(value).Type(TextQueryType.Phrase).Boost(10).Fields(f => 
                                        f.Field(f => f.LocationName)
                                    ))))));
            return responseElastic;
        }
        
        public async Task<ISearchResponse<Category>> CheckFieldExistCategory(string value)
        {
            ISearchResponse<Category> responseElastic;
            
            responseElastic = await _elasticClient.SearchAsync<Category>
            (
                s => s.Size(2000).Index(ElasticIndexConstant.CATEGORIES).Query(q =>
                    q.Bool(b =>
                        b.Should(
                            s =>
                                s.MultiMatch(t =>
                                    t.Query(value).Type(TextQueryType.Phrase).Boost(10).Fields(f => 
                                        f.Field(f => f.CategoryName)
                                    ))))));
            return responseElastic;
        }
        
        public async Task<ISearchResponse<Supplier>> CheckFieldExistSupplier(string value)
        {
            ISearchResponse<Supplier> responseElastic;
            
            responseElastic = await _elasticClient.SearchAsync<Supplier>
            (
                s => s.Size(2000).Index(ElasticIndexConstant.SUPPLIERS).Query(q =>
                    q.Bool(b =>
                        b.Should(
                            s =>
                                s.MultiMatch(t =>
                                    t.Query(value).Type(TextQueryType.Phrase).Boost(10).Fields(f => 
                                        f.Field(f => f.SupplierName).
                                            Field(f => f.Email)
                                    ))))));
            return responseElastic;
        }
    }
}