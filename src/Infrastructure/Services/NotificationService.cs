using System;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
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

        // public async Task SendNotification( string userId, string message)
        // {
        //     var notificationUserInfo = _notificationAsyncRepository.GetUserInfoFromUserId(userId);
        //     var notification = new Notification
        //     {
        //         Channel = notificationUserInfo.Channel,
        //         UserId = notificationUserInfo.UserId,
        //         Message = message,
        //         CreatedDate = DateTime.UtcNow,
        //         UserName = notificationUserInfo.UserName
        //     };
        //
        //     var isAdded = await _redisRepository.AddNotifications("Notifications", notification);
        //
        //     if (isAdded)
        //     {
        //         // await _hubContext.Clients.Groups(notification.Channel).SendAsync("NotificationGroupMessage", message);
        //         await _hubContext.Clients.All.SendAsync("NotificationGroupMessage", message);
        //     }
        //
        //     else
        //         throw new Exception();
        // }
        
        public async Task SendNotificationGroup(string groupName, string userId, string message, string type, string typeId)
        {
            var notificationUserInfo = _notificationAsyncRepository.GetUserInfoFromUserId(userId);
            var notification = new Notification
            {
                Channel = groupName,
                UserId = notificationUserInfo.Id,
                Message = message,
                CreatedDate = DateTime.UtcNow,
                UserName = notificationUserInfo.UserName,
                Type = type,
                TypeID = typeId
            };
    
            var isAdded = await _redisRepository.AddNotifications("Notifications", notification);

            if (isAdded)
                await _hubContext.Clients.Groups(groupName).SendAsync("NotificationGroupMessage", message);

            else
                throw new Exception();
        }

        public string CreateMessage(string fromUserFullname, string action, string page, string objectId)
        {
            return fromUserFullname + " " + action + " " + page + " with ID: " + objectId + ", at: " + DateTime.UtcNow;
        }
    }
}