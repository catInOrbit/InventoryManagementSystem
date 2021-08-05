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
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.SupplierEndpoints.Create
{
    public class SupplierCreate: BaseAsyncEndpoint.WithRequest<SupplierCreateRequest>.WithResponse<SupplierResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<Supplier> _supplierAsyncRepository;
        private readonly IElasticAsyncRepository<Supplier> _supplierIndexRepository;

        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;
        public SupplierCreate(IAsyncRepository<Supplier> supplierAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService, IElasticAsyncRepository<Supplier> supplierIndexRepository)
        {
            _supplierAsyncRepository = supplierAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _supplierIndexRepository = supplierIndexRepository;
        }
    
        [HttpPost("api/supplier/create")]
        [SwaggerOperation(
            Summary = "Create a new supplier",
            Description = "Create a new supplier",
            OperationId = "sup.create",
            Tags = new[] { "SupplierEndpoints" })
        ]
        public override async Task<ActionResult<SupplierResponse>> HandleAsync(SupplierCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.SUPPLIER, UserOperations.Create))
                return Unauthorized();

            var supplier = new Supplier
            {
                Address = request.Address,
                Description = request.Description,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                SalePersonName = request.SalePersonName,
                SupplierName = request.SupplierName 
            };
            
            supplier.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.Supplier, supplier.Id, (await _userAuthentication.GetCurrentSessionUser()).Id);
            supplier.LatestUpdateDate = DateTime.UtcNow;
            
            await _supplierAsyncRepository.AddAsync(supplier);
            await _supplierIndexRepository.ElasticSaveSingleAsync(true, supplier, ElasticIndexConstant.SUPPLIERS);

            
            // var currentUser = await _userAuthentication.GetCurrentSessionUser();
            //       
            // var messageNotification =
            //     _notificationService.CreateMessage(currentUser.Fullname, "Create","Supplier", supplier.Id);
            //     
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification, PageConstant.SUPPLIER, supplier.Id);

            var response = new SupplierResponse();
            response.ModifiedSupplierId = supplier.Id;
            return Ok(response);
        }
    }
    
    public class SupplierEdit: BaseAsyncEndpoint.WithRequest<SupplierUpdateRequest>.WithResponse<SupplierResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<Supplier> _supplierAsyncRepository;
        private readonly IElasticAsyncRepository<Supplier> _supplierIndexAsyncRepository;

        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;
        public SupplierEdit(IAsyncRepository<Supplier> supplierAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService, IElasticAsyncRepository<Supplier> supplierIndexAsyncRepository)
        {
            _supplierAsyncRepository = supplierAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _supplierIndexAsyncRepository = supplierIndexAsyncRepository;
        }
    
        [HttpPut("api/supplier/edit")]
        [SwaggerOperation(
            Summary = "Edit supplier",
            Description = "Edit supplier",
            OperationId = "sup.edit",
            Tags = new[] { "SupplierEndpoints" })
        ]
        public override async Task<ActionResult<SupplierResponse>> HandleAsync(SupplierUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.SUPPLIER, UserOperations.Update))
                return Unauthorized();
            
            var supplier = await _supplierAsyncRepository.GetByIdAsync(request.SupplierId);
            if (supplier == null)
                return NotFound("Can not find Supplier of id :" + request.SupplierId);
                                            
            supplier.Transaction = TransactionUpdateHelper.UpdateTransaction(supplier.Transaction,UserTransactionActionType.Modify,TransactionType.Supplier,
                (await _userAuthentication.GetCurrentSessionUser()).Id, supplier.Id, "");

            supplier.Description = request.Description;
            supplier.Email = request.Email;
            supplier.Address = request.Address;
            supplier.PhoneNumber = request.PhoneNumber;
            supplier.SupplierName = request.SupplierName;
            supplier.SalePersonName = request.SalePersonName;
            supplier.SupplierName = request.SupplierName;
            
            supplier.LatestUpdateDate = DateTime.UtcNow;

            await _supplierAsyncRepository.UpdateAsync(supplier);
            await _supplierIndexAsyncRepository.ElasticSaveSingleAsync(false, supplier, ElasticIndexConstant.SUPPLIERS);

            // var currentUser = await _userAuthentication.GetCurrentSessionUser();
            //       
            // var messageNotification =
            //     _notificationService.CreateMessage(currentUser.Fullname, "Update","Supplier", supplier.Id);
            //     
            // await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
            //     currentUser.Id, messageNotification);
            
            var response = new SupplierResponse();
            response.ModifiedSupplierId = supplier.Id;
            return Ok(response);
        }
    }
}