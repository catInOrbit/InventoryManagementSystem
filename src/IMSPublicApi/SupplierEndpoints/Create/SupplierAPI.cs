using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
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
    
        [HttpPost("api/supplier/create")]
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

            var supplier = request.Supplier;
            supplier.Transaction = new Transaction
            {
                Name = "Created New Supplier: " + supplier.Id,

                CreatedDate = DateTime.Now,
                Type = TransactionType.Supplier,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id,
                TransactionStatus = true
            };
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
    
    public class SupplierEdit: BaseAsyncEndpoint.WithRequest<SupplierUpdateRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<Supplier> _supplierAsyncRepository;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;
        public SupplierEdit(IAsyncRepository<Supplier> supplierAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService)
        {
            _supplierAsyncRepository = supplierAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
        }
    
        [HttpPut("api/supplier/edit")]
        [SwaggerOperation(
            Summary = "Edit supplier",
            Description = "Edit supplier",
            OperationId = "sup.edit",
            Tags = new[] { "SupplierEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(SupplierUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.SUPPLIER, UserOperations.Update))
                return Unauthorized();
            
            var supplier = await _supplierAsyncRepository.GetByIdAsync(request.SupplierId);

            supplier.Transaction.ModifiedDate = DateTime.Now;
            supplier.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            supplier.Transaction.Name = "Updated Supplier: " + supplier.Id;

            supplier.Description = request.Supplier.Description;
            supplier.Email = request.Supplier.Email;
            supplier.Address = request.Supplier.Address;
            supplier.PhoneNumber = request.Supplier.PhoneNumber;
            supplier.SupplierName = request.Supplier.SupplierName;
            supplier.SalePersonName = request.Supplier.SalePersonName;
            
            await _supplierAsyncRepository.UpdateAsync(supplier);
            await _supplierAsyncRepository.ElasticSaveSingleAsync(false, supplier, ElasticIndexConstant.SUPPLIERS);

            var currentUser = await _userAuthentication.GetCurrentSessionUser();
                  
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update","Supplier", request.Supplier.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);
            return Ok();
        }
    }
    
    
    //  public class SupplierDelete: BaseAsyncEndpoint.WithRequest<SupplierDeleteRequest>.WithoutResponse
    // {
    //     private readonly IAuthorizationService _authorizationService;
    //     private readonly IAsyncRepository<Supplier> _supplierAsyncRepository;
    //     private readonly IUserSession _userAuthentication;
    //     private readonly INotificationService _notificationService;
    //     public SupplierDelete(IAsyncRepository<Supplier> supplierAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService)
    //     {
    //         _supplierAsyncRepository = supplierAsyncRepository;
    //         _authorizationService = authorizationService;
    //         _userAuthentication = userAuthentication;
    //         _notificationService = notificationService;
    //     }
    //
    //     [HttpPut("api/supplier/deactivate/{Id}")]
    //     [SwaggerOperation(
    //         Summary = "Edit supplier",
    //         Description = "Edit supplier",
    //         OperationId = "sup.edit",
    //         Tags = new[] { "SupplierEndpoints" })
    //     ]
    //     public override async Task<ActionResult> HandleAsync([FromRoute]SupplierDeleteRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.SUPPLIER, UserOperations.Delete))
    //             return Unauthorized();
    //
    //         var supplierToDelete = await _supplierAsyncRepository.GetByIdAsync(request.Id); 
    //         await _supplierAsyncRepository.DeleteAsync(supplierToDelete);
    //         await _supplierAsyncRepository.ElasticDeleteSingleAsync(supplierToDelete, ElasticIndexConstant.SUPPLIERS);
    //
    //         var currentUser = await _userAuthentication.GetCurrentSessionUser();
    //               
    //         var messageNotification =
    //             _notificationService.CreateMessage(currentUser.Fullname, "Delete","Supplier", request.Id);
    //             
    //         await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
    //             currentUser.Id, messageNotification);
    //         return Ok();
    //     }
    // }
}