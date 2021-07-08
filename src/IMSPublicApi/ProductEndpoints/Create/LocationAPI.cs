using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Create
{
    public class LocationCreate : BaseAsyncEndpoint.WithRequest<LocationCreateRequest>.WithResponse<LocationResponse>
    {
        private readonly IAsyncRepository<Location> _locationAsyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;

        public LocationCreate(IAsyncRepository<Location> locationAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService)
        {
            _locationAsyncRepository = locationAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
        }
        
        [HttpPost("api/location/create")]
        [SwaggerOperation(
            Summary = "Create a new location",
            Description = "Create a new location",
            OperationId = "product_location.create",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<LocationResponse>> HandleAsync(LocationCreateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Create))
                return Unauthorized();
            
            
            var location = new Location
            {
                LocationName = request.LocationName
            };

            if (request.LocationBarcode != null)
                location.LocationBarcode = request.LocationBarcode;

            var userId = (await _userAuthentication.GetCurrentSessionUser()).Id;
            location.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.Location, location.Id,
                userId);

            await _locationAsyncRepository.AddAsync(location);
            
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create", PageConstant.PRODUCT_LOCATION, location.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            return Ok(new LocationResponse
            {
                Location = location
            });
        }
    }
    
     public class LocationUpdate : BaseAsyncEndpoint.WithRequest<LocationUpdateRequest>.WithResponse<LocationResponse>
    {
        private readonly IAsyncRepository<Location> _locationAsyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;

        public LocationUpdate(IAsyncRepository<Location> locationAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService)
        {
            _locationAsyncRepository = locationAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
        }
        
        [HttpPut("api/location/update")]
        [SwaggerOperation(
            Summary = "Create a new location",
            Description = "Create a new location",
            OperationId = "product_location.create",
            Tags = new[] { "ProductEndpoints" })
        ]
        public override async Task<ActionResult<LocationResponse>> HandleAsync(LocationUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PRODUCT, UserOperations.Create))
                return Unauthorized();

            var location = await _locationAsyncRepository.GetByIdAsync(request.LocationId);
            if (location == null)
                return NotFound("Location not found");

            location.LocationName = request.LocationName;
            location.LocationBarcode = request.LocationBarcode;
            
            var userId = (await _userAuthentication.GetCurrentSessionUser()).Id;
            location.Transaction = TransactionUpdateHelper.UpdateTransaction(location.Transaction, UserTransactionActionType.Modify, userId, location.Id, "");
            
            await _locationAsyncRepository.UpdateAsync(location);
                        
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update", PageConstant.PRODUCT_LOCATION, location.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification);

            return Ok(new LocationResponse
            {
                Location = location
            });
        }
    }
}