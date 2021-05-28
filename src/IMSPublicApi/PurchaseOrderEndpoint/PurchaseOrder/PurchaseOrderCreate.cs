// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Ardalis.ApiEndpoints;
// using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder
// {
//     [Authorize]
//     public class PurchaseOrderCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<PurchaseOrderCreateResponse>
//     {
//         private readonly IAuthorizationService _authorizationService;
//
//         public PurchaseOrderCreate(IAuthorizationService authorizationService)
//         {
//             _authorizationService = authorizationService;
//         }
//
//         public override async Task<ActionResult<PurchaseOrderCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
//         {
//             var response = new PurchaseOrderCreateResponse();
//
//             var isAuthorized = await _authorizationService.AuthorizeAsync(
//                 HttpContext.User, "PurchaseOrder",
//                 UserOperations.Create);
//
//             response.PurchaseOrder = new ApplicationCore.Entities.Orders.PurchaseOrder();
//
//             return Ok(response);
//         }
//     }
// }