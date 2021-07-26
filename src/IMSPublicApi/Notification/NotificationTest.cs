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
                pagingOptionPackage.ResultList = notifications.OrderByDescending(n => n.CreatedDate).ToList();
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