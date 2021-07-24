using System;
using System.Collections.Generic;
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
        private IRedisRepository _redisRepository;
        public GetNotification(IAsyncRepository<Notification> notificationAsyncRepository, IRedisRepository redisRepository)
        {
            _notificationAsyncRepository = notificationAsyncRepository;
            _redisRepository = redisRepository;
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

            List<Notification> notifications = new List<Notification>();
            var response = new NotificationGetResponse();
            if (request.Channel == null)
                return NotFound("Channel not found! Specify channel");
            
            response.IsDisplayingAll = true;
            try
            {
                notifications.AddRange(await _redisRepository.GetNotificationAllByChannel("Notifications", request.Channel));
                notifications.AddRange(await _notificationAsyncRepository.ListAllNotificationByChannel(request.Channel));
                pagingOptionPackage.ResultList = notifications;
                pagingOptionPackage.ExecuteResourcePaging();

                response.Paging = pagingOptionPackage;
                return Ok(response);
            }
            catch
            {
                return NotFound("Error getting notifications, recheck channel");
            }
        }
    }
}