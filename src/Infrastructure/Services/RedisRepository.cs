using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Notifications;
using StackExchange.Redis;

namespace Infrastructure.Services
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase database;

        public RedisRepository(ConnectionMultiplexer redis)
        {
            database = redis.GetDatabase();
        }

        public async Task<bool> AddGroup(NotificationUserGroup group)
        {
            var isDone = await database.StringSetAsync(group.Name.ToString(), JsonSerializer.Serialize(group));
            return isDone;
        }

        public async Task<bool> AddUserToGroup(string groupName, NotificationUser user)
        {
            var data = await database.StringGetAsync(groupName.ToString());

            if (data.IsNullOrEmpty)
                return false;

            var group = JsonSerializer.Deserialize<NotificationUserGroup>(data);
            group.NotificationUsers.Add(user);

            return await AddGroup(group);
        }

        public async Task<List<NotificationUser>> GetUsersFromGroup(string groupid)
        {
            var data = await database.StringGetAsync(groupid.ToString());

            if (data.IsNullOrEmpty)
                return new List<NotificationUser>();

            var group = JsonSerializer.Deserialize<NotificationUserGroup>(data);
            return group.NotificationUsers.ToList();
        }

        public async Task<bool> RemoveUserFromGroup(string keyId, string userId)
        {
            var data = await database.StringGetAsync(keyId.ToString());

            if (data.IsNullOrEmpty)
                return false;

            var group = JsonSerializer.Deserialize<NotificationUserGroup>(data);
            group.NotificationUsers.RemoveAll(u => u.Id == userId);
            return await AddGroup(group);
        }

        //make notification
        public async Task<bool> AddNotifications(string keyId, Notification notification)
        {
            var data = await database.StringGetAsync(keyId);

            if (data.IsNullOrEmpty)
            {
                var groupNotificationNew = new GroupNotifications
                {
                    Id = "Notifications",
                    Name = "Notifications",
                    Description = "Notifications",
                };
                
                await database.StringSetAsync(keyId, JsonSerializer.Serialize(groupNotificationNew));
            }
            
            data = await database.StringGetAsync(keyId);

            var group_notification = JsonSerializer.Deserialize<GroupNotifications>(data);
            group_notification.Notifications.Add(notification);


            //var isDone = await database.StringSetAsync(group.Id.ToString(), JsonSerializer.Serialize(group));
            //return isDone;
            return await database.StringSetAsync(keyId, JsonSerializer.Serialize(group_notification));
        }

        public async Task<List<Notification>> GetNotificationAll(string keyId)
        {
            var data = await database.StringGetAsync(keyId);

            if (data.IsNullOrEmpty)
                return new List<Notification>();


            var notifications_manager = JsonSerializer.Deserialize<GroupNotifications>(data);
            return notifications_manager.Notifications;
        }
    }

}