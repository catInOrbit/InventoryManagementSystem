// using System;
// using System.Threading.Tasks;
// using InventoryManagementSystem.ApplicationCore.Entities;
// using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
// using InventoryManagementSystem.PublicApi.ProductEndpoints.Product;
// using Moq;
// using Nest;
// using Xunit;
//
// namespace IMSBackend.Tests
// {
//     public class SearchTest
//     {
//         private readonly IElasticClient _elasticClient;
//
//         public SearchTest(IElasticClient elasticClient)
//         {
//             _elasticClient = elasticClient;
//         }
//
//
//         [Theory]
//         [InlineData("Lom", 1, 5)]
//         public async Task ProductSearchSingleTest(string query, int currentPage, int sizePerPage)
//         {
//             
//             var elasticService = new Mock<IElasticClient>();
//             SearchDescriptor<ProductSearchIndex> searchDescriptor = new SearchDescriptor<ProductSearchIndex>();
//             searchDescriptor.Index("productindices");
//             QueryContainerDescriptor<ProductSearchIndex> queryContainerDescriptor =
//                 new QueryContainerDescriptor<ProductSearchIndex>();
//
//             QueryStringQueryDescriptor<ProductSearchIndex> queryStringQueryDescriptor =
//                 new QueryStringQueryDescriptor<ProductSearchIndex>();
//             queryStringQueryDescriptor.Query('*' + query+ '*');
//             
//             
//             elasticService.Setup(e => e.SearchAsync<ProductSearchIndex>
//             (
//                 s => s.Index("productindices"));
//             
//             
//             PagingOption<ProductSearchIndex> pagingOption =
//                 new PagingOption<ProductSearchIndex>(currentPage, sizePerPage);
//             var responseElastic = await _elasticClient.SearchAsync<ProductSearchIndex>
//             (
//                 s => s.Index("productindices").Query(q =>q.QueryString(d =>d.Query('*' + query + '*'))));
//             
//             foreach (var productSearchIndex in responseElastic.Documents)
//                 pagingOption.ResultList.Add(productSearchIndex);
//             
//             pagingOption.ExecuteResourcePaging();
//             
//             var response = new GetProductSearchResponse();
//             response.Paging = pagingOption;
//             Assert.True(response.Paging.ResultList.Count >=1);
//         }
//  
//         
//         public void PurchaseOrderSearchTest()
//         {
//             
//         }
//         
//         public void GoodsReceiptSearchTest()
//         {
//             
//         }
//         
//         public void RequisitionSearchTest()
//         {
//             
//         }
//         
//         public void StockTakingSearchTest()
//         {
//             
//         }
//         
//         public void GoodsIssueSearchTest()
//         {
//             
//         }
//         
//         
//     }
// }