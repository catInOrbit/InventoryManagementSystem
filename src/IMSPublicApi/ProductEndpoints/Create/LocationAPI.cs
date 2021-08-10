using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Location
{
    public class LocationCreate : BaseAsyncEndpoint.WithRequest<LocationCreateRequest>.WithResponse<LocationResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Products.Location> _locationAsyncRepository;
        private readonly IElasticAsyncRepository<ApplicationCore.Entities.Products.Location> _locationEls;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;

        public LocationCreate(IAsyncRepository<ApplicationCore.Entities.Products.Location> locationAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService, IElasticAsyncRepository<ApplicationCore.Entities.Products.Location> locationEls)
        {
            _locationAsyncRepository = locationAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _locationEls = locationEls;
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
            
            
            var location = new ApplicationCore.Entities.Products.Location
            {
                LocationName = request.LocationName,
                LatestUpdateDate = DateTime.UtcNow
            };

            location.LocationBarcode = "LO" + Guid.NewGuid().ToString().Substring(0, 10);

            var userId = (await _userAuthentication.GetCurrentSessionUser()).Id;
            location.Transaction = TransactionUpdateHelper.CreateNewTransaction(TransactionType.Location, location.Id,
                userId);

            await _locationAsyncRepository.AddAsync(location);
            
            await _locationEls.ElasticSaveSingleAsync(false, location, ElasticIndexConstant.LOCATIONS);
            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Create", "Location", location.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification,PageConstant.LOCATION, location.Id);

            return Ok(new LocationResponse
            {
                Location = location
            });
        }
    }
    
     public class LocationUpdate : BaseAsyncEndpoint.WithRequest<LocationUpdateRequest>.WithResponse<LocationResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Products.Location> _locationAsyncRepository;
        private readonly IElasticAsyncRepository<ApplicationCore.Entities.Products.Location> _locationEls;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userAuthentication;
        private readonly INotificationService _notificationService;

        public LocationUpdate(IAsyncRepository<ApplicationCore.Entities.Products.Location> locationAsyncRepository, IAuthorizationService authorizationService, IUserSession userAuthentication, INotificationService notificationService, IElasticAsyncRepository<ApplicationCore.Entities.Products.Location> locationEls)
        {
            _locationAsyncRepository = locationAsyncRepository;
            _authorizationService = authorizationService;
            _userAuthentication = userAuthentication;
            _notificationService = notificationService;
            _locationEls = locationEls;
        }
        
        [HttpPut("api/location/update")]
        [SwaggerOperation(
            Summary = "Create a new location",
            Description = "Create a new location",
            OperationId = "product_location.update",
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
            location.LatestUpdateDate = DateTime.UtcNow;
            var userId = (await _userAuthentication.GetCurrentSessionUser()).Id;
            location.Transaction = TransactionUpdateHelper.UpdateTransaction(location.Transaction, UserTransactionActionType.Modify,TransactionType.Location, userId, location.Id, "");
            
            await _locationAsyncRepository.UpdateAsync(location);
            await _locationEls.ElasticSaveSingleAsync(false, location, ElasticIndexConstant.LOCATIONS);

            var currentUser = await _userAuthentication.GetCurrentSessionUser();
            
            var messageNotification =
                _notificationService.CreateMessage(currentUser.Fullname, "Update", "LOCATION", location.Id);
                
            await _notificationService.SendNotificationGroup(await _userAuthentication.GetCurrentSessionUserRole(),
                currentUser.Id, messageNotification, PageConstant.LOCATION, location.Id);

            return Ok(new LocationResponse
            {
                Location = location
            });
        }
    }
}