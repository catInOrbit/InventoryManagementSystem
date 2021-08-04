using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Notifications;
using InventoryManagementSystem.ApplicationCore.Entities.RedisMessages;
using InventoryManagementSystem.ApplicationCore.Extensions;
using InventoryManagementSystem.ApplicationCore.Interfaces;
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

        public async Task<List<Notification>> GetNotificationAllByChannel(string keyId, string channel)
        {
            var data = await database.StringGetAsync(keyId);

            if (data.IsNullOrEmpty)
                return new List<Notification>();
            var notifications_manager = JsonSerializer.Deserialize<GroupNotifications>(data);
            notifications_manager.Notifications =
                notifications_manager.Notifications.Where(n => n.Channel == channel).ToList();
            
            return notifications_manager.Notifications;
        }

        public async Task ClearNotification(string keyId)
        {
            var data = await database.StringGetAsync(keyId);
            var group_notification = JsonSerializer.Deserialize<GroupNotifications>(data);
            group_notification.Notifications.Clear();
            
            await database.StringSetAsync(keyId, JsonSerializer.Serialize(group_notification));
        }

        public async Task<bool> AddProductUpdateMessage( ProductUpdateMessage productUpdateMessage)
        {
            var data = await database.StringGetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE);

            if (data.IsNullOrEmpty)
            {
                var productMessageGroup = new ProductMessageGroup
                {
                    Id = RedisConstants.REDIS_PRODUCTUPDATEMESSAGE,
                    ModifiedDate = DateTime.UtcNow,
                    ProductUpdateMessages =  new List<ProductUpdateMessage>()
                };
                
                await database.StringSetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE, JsonSerializer.Serialize(productMessageGroup));
            }
            
            data = await database.StringGetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE);

            var productGroup = JsonSerializer.Deserialize<ProductMessageGroup>(data);
            productGroup.ProductUpdateMessages.Add(productUpdateMessage);


            //var isDone = await database.StringSetAsync(group.Id.ToString(), JsonSerializer.Serialize(group));
            //return isDone;
            return await database.StringSetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE, JsonSerializer.Serialize(productGroup));
            await database.StringSetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE, JsonSerializer.Serialize(productUpdateMessage));
        }

        public async Task<bool> AddStockTakeAdjustMessage(StockTakeAdjustItemInfo stockTakeAdjustItemInfo)
        {
            var data = await database.StringGetAsync("StockTakeMessage");
            if (data.IsNullOrEmpty)
            {
                var stockTakeInfo = new StockTakeAdjustInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    StockTakeAdjustItemsInfos = new List<StockTakeAdjustItemInfo>()
                };
                
                await database.StringSetAsync("StockTakeMessage", JsonSerializer.Serialize(stockTakeInfo));
            }
            
            data = await database.StringGetAsync("StockTakeMessage");
            
            var stockTakeMessage = JsonSerializer.Deserialize<StockTakeAdjustInfo>(data);
            
            if(!stockTakeMessage.StockTakeAdjustItemsInfos.Contains(stockTakeAdjustItemInfo))
                stockTakeMessage.StockTakeAdjustItemsInfos.Add(stockTakeAdjustItemInfo);
            
            return await database.StringSetAsync("StockTakeMessage", JsonSerializer.Serialize(stockTakeMessage));
        }

        public async Task<List<StockTakeAdjustItemInfo>> GetStockTakeAdjustMessage()
        {
            var data = await database.StringGetAsync("StockTakeMessage");
            var message = JsonSerializer.Deserialize<StockTakeAdjustInfo>(data);
            return message.StockTakeAdjustItemsInfos;
        }

        public async Task<bool> DeleteStockTakeAdjustMessage()
        {
            var data = await database.StringGetAsync("StockTakeMessage");
            var message = JsonSerializer.Deserialize<StockTakeAdjustInfo>(data);
            message.StockTakeAdjustItemsInfos.Clear();
            
            return await database.StringSetAsync("StockTakeMessage", JsonSerializer.Serialize(message));
        }


        public async Task<bool> RemoveProductUpdateMessage(string productVariantId)
        {
            var data = await database.StringGetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE);
            if (data.IsNullOrEmpty)
                throw new Exception("Unable to find redis key : " + RedisConstants.REDIS_PRODUCTUPDATEMESSAGE + ". Key may not exist");
            
            var productGroupMessage = JsonSerializer.Deserialize<ProductMessageGroup>(data);
            var itemToRemove =
                productGroupMessage.ProductUpdateMessages
                    .FirstOrDefault(p => p.ProductVariantId == productVariantId);
            productGroupMessage.ProductUpdateMessages.Remove(itemToRemove);
            return await database.StringSetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE, JsonSerializer.Serialize(productGroupMessage));
        }

        public async Task<List<ProductUpdateMessage>> GetProductUpdateMessage()
        {
            
            var data = await database.StringGetAsync(RedisConstants.REDIS_PRODUCTUPDATEMESSAGE);

            var productGroupMessage = JsonSerializer.Deserialize<ProductMessageGroup>(data);
            return productGroupMessage.ProductUpdateMessages;
        
        }
    }

}