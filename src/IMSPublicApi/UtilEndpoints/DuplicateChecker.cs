// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Ardalis.ApiEndpoints;
// using InventoryManagementSystem.ApplicationCore.Entities;
// using Microsoft.AspNetCore.Mvc;
// using Nest;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace InventoryManagementSystem.PublicApi.UtilEndpoints
// {
//     public class DuplicateChecker : BaseAsyncEndpoint.WithRequest<DuplicateCheckerRequest>.WithResponse<DuplicateCheckerResponse>
//     {
//         private readonly IElasticClient _elasticClient;
//
//         public DuplicateChecker(IElasticClient elasticClient)
//         {
//             _elasticClient = elasticClient;
//         }
//         [HttpGet("api/duplicatecheck")]
//         [SwaggerOperation(
//             Summary = "Search Product Variant",
//             Description = "Search Product Variant",
//             OperationId = "product.searchvariants",
//             Tags = new[] { "ProductEndpoints" })
//         ]
//         public override async Task<ActionResult<DuplicateCheckerResponse>> HandleAsync([FromQuery]DuplicateCheckerRequest request, CancellationToken cancellationToken = new CancellationToken())
//         {
//             var responseElastic = await _elasticClient.SearchAsync<BaseEntity>
//             (
//                 s => s.Size(2000).AllIndices().Query(q => q.Match(iq => iq.Query(request.SearchQuery))));
//
//             foreach (var responseElasticDocument in responseElastic.Documents)
//             {
//                 Console.WriteLine(responseElasticDocument.Id);
//             }
//             return Ok();
//         }
//     }
// }