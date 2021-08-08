using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.PublicApi.SuggestionSearchSchema;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class PurchaseOrderSearchSugesstion : BaseAsyncEndpoint.WithRequest<SearchSuggestionRequest>.WithResponse<SearchSuggestionResponse>
    {
        private readonly IElasticClient _elasticClient;

        public PurchaseOrderSearchSugesstion(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        [HttpGet("api/po/els/{Query}")]
        [SwaggerOperation(
            Summary = "Elastic")
        ]

        public override async Task<ActionResult<SearchSuggestionResponse>> HandleAsync([FromRoute]SearchSuggestionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>
            (
                s => s.Index( ElasticIndexConstant.PURCHASE_ORDERS).
                    Suggest(su 
                        => su.Completion("suggestions", c 
                            => c.Field(f=>f.Suggest).
                                Prefix(request.Query).
                                Fuzzy(f => f.Fuzziness(Fuzziness.Auto)).Size(5)))
                    .Query(q => q.QueryString(d => d.Query('*' + request.Query + '*'))).From(0).Size(6));
            var response = new SearchSuggestionResponse();
            foreach (var purchaseOrderSearchIndex in responseElastic.Documents)
                response.Suggestions.AddRange(purchaseOrderSearchIndex.Suggest.Input);
            return Ok(response);
        }
    }
    
    public class ProductSearchSugesstion : BaseAsyncEndpoint.WithRequest<SearchSuggestionRequest>.WithResponse<SearchSuggestionResponse>
    {
        private readonly IElasticClient _elasticClient;

        public ProductSearchSugesstion(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        [HttpGet("api/product/els/{Query}")]
        [SwaggerOperation(
            Summary = "Elastic")
        ]

        public override async Task<ActionResult<SearchSuggestionResponse>> HandleAsync([FromRoute]SearchSuggestionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var responseElastic = await _elasticClient.SearchAsync<ProductVariantSearchIndex>
            (
                s => s.Index( ElasticIndexConstant.PRODUCT_INDICES).
                    Suggest(su 
                        => su.Completion("suggestions", c 
                            => c.Field(f=>f.Suggest).
                                Prefix(request.Query).
                                Fuzzy(f => f.Fuzziness(Fuzziness.Auto)).Size(5)))
                    .Query(q 
                        => q.MultiMatch(qs => qs.Fields(d
                            => d.Field(f => f.Name)
                           ).Type(TextQueryType.BoolPrefix).Query(request.Query))).From(0).Size(6));
            var response = new SearchSuggestionResponse();
            foreach (var purchaseOrderSearchIndex in responseElastic.Documents)
                response.Suggestions.AddRange(purchaseOrderSearchIndex.Suggest.Input);
            return Ok(response);
        }
    }
}