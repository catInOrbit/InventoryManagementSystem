using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        
        
        private IHubContext<NotificationHub> _hubContext;
        private readonly IRedisRepository _redisRepository;
        private readonly IAsyncRepository<Notification> _notificationAsyncRepository;

        public NotificationService(IRedisRepository redisRepository, IAsyncRepository<Notification> notificationAsyncRepository, IHubContext<NotificationHub> hubContext)
        {
            _redisRepository = redisRepository;
            _notificationAsyncRepository = notificationAsyncRepository;
            _hubContext = hubContext;
        }
        
        public NotificationService()
        {
        }


        public async Task SendNotification( string userId, string message)
        {
            
            var notificationInfo = _notificationAsyncRepository.GetNotificationInfoFromUserId(userId);
            var notification = new Notification
            {
                Channel = notificationInfo.Channel,
                UserId = notificationInfo.UserId,
                Message = message,
                CreatedDate = DateTime.UtcNow,
                UserName = notificationInfo.UserName
            };

            var isAdded = await _redisRepository.AddNotifications("Notifications", notification);

            if (isAdded)
            {
                // await _hubContext.Clients.Groups(notification.Channel).SendAsync("NotificationGroupMessage", message);
                await _hubContext.Clients.All.SendAsync("NotificationGroupMessage", message);
            }

            else
                throw new Exception();
        }
        
        public async Task SendNotificationGroup(string groupName, string userId, string message)
        {
            
            var notificationInfo = _notificationAsyncRepository.GetNotificationInfoFromUserId(userId);
            var notification = new Notification
            {
                Channel = notificationInfo.Channel,
                UserId = notificationInfo.UserId,
                Message = message,
                CreatedDate = DateTime.UtcNow,
                UserName = notificationInfo.UserName
            };

            var isAdded = await _redisRepository.AddNotifications("Notifications", notification);

            if (isAdded)
            {
                await _hubContext.Clients.Groups(groupName).SendAsync("NotificationGroupMessage", message);
                // await _hubContext.Clients.All.SendAsync("NotificationGroupMessage", message);
            }

            else
                throw new Exception();
        }

        public string CreateMessage(string fromUserFullname, string action, string page, string objectId)
        {
            return fromUserFullname + " " + action + " " + page + " with ID: " + objectId + ", at: " + DateTime.UtcNow;
        }
    }
}