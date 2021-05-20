using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.StockkeeperEndpoints
{
    // public class AddProduct : BaseAsyncEndpoint.WithRequest<>.WithResponse<>,
    // {
    //
    //     private readonly AppIdentityDbContext _appIdentityDbContext;
    //     private readonly IAuthorizationService _authorizationService;
    //     private readonly UserManager<IdentityUser> _userManager;
    //
    //     public AddProduct(
    //         AppIdentityDbContext context,
    //         IAuthorizationService authorizationService,
    //         UserManager<IdentityUser> userManager)
    //     {
    //         _appIdentityDbContext = context;
    //         _authorizationService = authorizationService;
    //         _userManager = userManager;
    //     }
    //     public IMSUser ImsUser { get; set; }
    //
    //     // public override async Task<ActionResult<CreateCatalogItemResponse>> HandleAsync(
    //     //     AddProductRequest request, CancellationToken cancellationToken)
    //     // {
    //     //     var isAuthorized = await _authorizationService.AuthorizeAsync(
    //     //         User, ImsUser,
    //     //         ContactOperations.Create);
    //     //
    //     //     if (isAuthorized.Succeeded)
    //     //     {
    //     //         
    //     //     }
    //     //     
    //     //     //Add new product to DBcontext --> engage migration
    //     //     Context
    //     // }
    //
    // }
}