// using System.Threading;
// using System.Threading.Tasks;
// using Ardalis.ApiEndpoints;
// using Infrastructure.Services;
// using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
// {
//     public class ReceivingOrderUpdateItem : BaseAsyncEndpoint.WithRequest<ROUpdateItemRequest>.WithoutResponse
//     {
//         private readonly IAuthorizationService _authorizationService;
//
//         public ReceivingOrderUpdateItem(IAuthorizationService authorizationService)
//         {
//             _authorizationService = authorizationService;
//         }
//
//         public override async Task<ActionResult> HandleAsync(ROUpdateItemRequest request, CancellationToken cancellationToken = new CancellationToken())
//         {
//             if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
//                 return Unauthorized();
//             return null;
//         }
//     }
// }