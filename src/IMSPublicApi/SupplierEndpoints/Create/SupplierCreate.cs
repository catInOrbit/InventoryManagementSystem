using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.Create
{
    public class SupplierCreate: BaseAsyncEndpoint.WithRequest<SupplierCreateRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<Supplier> _supplierAsyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;
        public SupplierCreate(IAsyncRepository<Supplier> supplierAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService)
        {
            _supplierAsyncRepository = supplierAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
        }
    
        [HttpPost("api/supplier/create/")]
        [SwaggerOperation(
            Summary = "Create a new supplier",
            Description = "Create a new supplier",
            OperationId = "sup.create",
            Tags = new[] { "SupplierEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(SupplierCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.SUPPLIER, UserOperations.Create))
                return Unauthorized();
            
            await _supplierAsyncRepository.AddAsync(request.Supplier);
            await _supplierAsyncRepository.ElasticSaveSingleAsync(true, request.Supplier, ElasticIndexConstant.SUPPLIERS);

            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                  
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create","Supplier", request.Supplier.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok();
        }
    }
}