// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Ardalis.ApiEndpoints;
// using Infrastructure.Services;
// using InventoryManagementSystem.ApplicationCore.Entities;
// using InventoryManagementSystem.ApplicationCore.Entities.Orders;
// using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
// using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
// using InventoryManagementSystem.ApplicationCore.Interfaces;
// using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote
// {
//     public class GetPriceQuote : BaseAsyncEndpoint.WithRequest<GetPriceQuoteRequest>.WithResponse<GetPriceQuoteResponse>
//     {
//         private IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
//         private IAsyncRepository<PqDisplay> _pqDisplayAsyncRepository;
//
//         private readonly IAuthorizationService _authorizationService;
//
//         public GetPriceQuote(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService)
//         {
//             _asyncRepository = asyncRepository;
//             _authorizationService = authorizationService;
//         }
//         
//         [HttpGet("api/pricequote/{Number}&page={CurrentPage}&size={SizePerPage}")]
//         [SwaggerOperation(
//             Summary = "Get all price quote",
//             Description = "Get all price quote, {Number} = all to get all or search for a specific price quote",
//             OperationId = "po.update",
//             Tags = new[] { "PriceQuoteOrderEndpoints" })
//         ]
//         public override async Task<ActionResult<GetPriceQuoteResponse>> HandleAsync([FromRoute] GetPriceQuoteRequest request, CancellationToken cancellationToken = new CancellationToken())
//         {
//             // var isAuthorized = await _authorizationService.AuthorizeAsync(
//             //     HttpContext.User, "PurchaseOrder",
//             //     UserOperations.Read);
//             //
//             // if (!isAuthorized.Succeeded)
//             //     return Unauthorized();
//             
//             if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PriceQuoteOrder", UserOperations.Read))
//                 return Unauthorized();
//             
//             var response = new GetPriceQuoteResponse();
//
//             PagingOption<ApplicationCore.Entities.Orders.PurchaseOrder> pagingOption = new PagingOption<ApplicationCore.Entities.Orders.PurchaseOrder>(
//                 request.CurrentPage, request.SizePerPage);
//             
//             if (request.Number == "all")
//             {
//                 response.IsForDisplay = true;
//                 var po = await _asyncRepository.ListAllAsync(pagingOption, cancellationToken);
//                 pagingOption.RowCountTotal = po.ResultList.Count;
//                 foreach (var priceQuoteOrder in po.ResultList)
//                 {
//                     if (CheckFormDisplayable(priceQuoteOrder))
//                     {
//                         var pq = new PqDisplay
//                         {
//                             Id = priceQuoteOrder.Id,
//                             Deadline = priceQuoteOrder.Deadline.ToString("MM/dd/yyyy"),
//                             CreatedDate = priceQuoteOrder.Transaction.CreatedDate.ToString("MM/dd/yyyy"),
//                             CreatedByName = priceQuoteOrder.Transaction.CreatedBy.Fullname,
//                             OrderNumber = priceQuoteOrder.PurchaseOrderNumber,
//                             SupplierName = (priceQuoteOrder.Supplier != null) ? priceQuoteOrder.Supplier.SupplierName : "",
//                         };
//                         
//                         foreach (var purchaseOrderItem in priceQuoteOrder.PurchaseOrderProduct)
//                         {
//                             pq.NumberOfProduct += 1 ;
//                         }
//                         response.PriceQuotes.Add(pq);
//                     }
//                 }
//                 
//             }
//
//             else
//             {
//                 response.IsForDisplay = false;
//                 // var pos = await _asyncRepository.ListAllAsync(cancellationToken);
//                 // var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>(
//                 //     s => s.Query(q => q.QueryString(d => d.Query('*' + request.number + '*'))));
//                 response.PriceQuoteOrder = _asyncRepository.GetPurchaseOrderByNumber(request.Number, cancellationToken);
//             }
//             return Ok(response);
//         }
//
//         private bool CheckFormDisplayable(ApplicationCore.Entities.Orders.PurchaseOrder purchaseOrder)
//         {
//             if (purchaseOrder.PurchaseOrderStatus == PurchaseOrderStatusType.PQCreated ||
//                 purchaseOrder.PurchaseOrderStatus == PurchaseOrderStatusType.PQSent)
//                 return true;
//             return false;
//         }
//     }
// }