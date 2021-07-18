using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi
{
    
    public class NotificationTest : BaseAsyncEndpoint.WithRequest<NotificationTestRequest>.WithoutResponse
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IAsyncRepository<Notification> _notificationAsyncRepository;
        private IHubContext<NotificationHub> _hubContext;

        private readonly IUserSession _userAuthentication;

        
        public NotificationTest(IRedisRepository redisRepository, IAsyncRepository<Notification> notificationAsyncRepository, IHubContext<NotificationHub> hubContext, IUserSession userAuthentication)
        {
            _redisRepository = redisRepository;
            _notificationAsyncRepository = notificationAsyncRepository;
            _hubContext = hubContext;
            _userAuthentication = userAuthentication;
        }
        
        [HttpPost("api/notification")]
        [SwaggerOperation(
            Summary = "Notification Test",
            Description = "Notification Test",
            OperationId = "notitest",
            Tags = new[] { "NotificationTest" })
        ]
        public override async Task<ActionResult> HandleAsync(NotificationTestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var notificationService = new NotificationService(_redisRepository, _notificationAsyncRepository, _hubContext);

            await notificationService.SendNotification(request.UserId, request.Message);
            return Ok();
        }
    }
    
    public class NotificationGroupTest : BaseAsyncEndpoint.WithRequest<NotificationGroupTestRequest>.WithoutResponse
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IAsyncRepository<Notification> _notificationAsyncRepository;
        private IHubContext<NotificationHub> _hubContext;

        private readonly IUserSession _userAuthentication;

        public NotificationGroupTest(IRedisRepository redisRepository, IAsyncRepository<Notification> notificationAsyncRepository, IHubContext<NotificationHub> hubContext, IUserSession userAuthentication)
        {
            _redisRepository = redisRepository;
            _notificationAsyncRepository = notificationAsyncRepository;
            _hubContext = hubContext;
            _userAuthentication = userAuthentication;
        }

        [HttpPost("api/notificationgroup")]
        [SwaggerOperation(
            Summary = "Notification Test",
            Description = "Notification Test",
            OperationId = "notitest",
            Tags = new[] { "NotificationTest" })
        ]
        public override async Task<ActionResult> HandleAsync(NotificationGroupTestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var notificationService = new NotificationService(_redisRepository, _notificationAsyncRepository, _hubContext);
            
            await notificationService.SendNotificationGroup(request.Group, request.UserId, request.Message);
            return Ok();
        }
    }
    
    public class GetNotification : BaseAsyncEndpoint.WithRequest<NotificationGetRequest>.WithResponse<NotificationGetResponse>
    {
        private readonly IAsyncRepository<Notification> _notificationAsyncRepository;

        public GetNotification(IAsyncRepository<Notification> notificationAsyncRepository)
        {
            _notificationAsyncRepository = notificationAsyncRepository;
        }

        [HttpGet("api/notification")]
        [SwaggerOperation(
            Summary = "Get Notification by latest date",
            Description = "Get Notification by latest date",
            OperationId = "noti",
            Tags = new[] { "Notification" })
        ]
        public override async Task<ActionResult<NotificationGetResponse>> HandleAsync([FromQuery] NotificationGetRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            
            PagingOption<Notification> pagingOptionPackage =
                new PagingOption<Notification>(request.CurrentPage, request.SizePerPage);

            var response = new NotificationGetResponse();
            response.IsDisplayingAll = true;
            response.Paging = await _notificationAsyncRepository.ListAllAsync(pagingOptionPackage, cancellationToken);
            response.Paging.ResultList = response.Paging.ResultList.OrderByDescending(no => no.CreatedDate).ToList();
            
            return Ok(response);
        }
    }
}