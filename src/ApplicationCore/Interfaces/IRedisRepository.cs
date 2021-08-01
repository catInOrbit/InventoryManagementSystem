using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Notifications;
using InventoryManagementSystem.ApplicationCore.Entities.RedisMessages;
using InventoryManagementSystem.ApplicationCore.Extensions;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IRedisRepository
    {
        Task<bool> AddGroup(NotificationUserGroup group);

        Task<bool> AddUserToGroup(string groupid, NotificationUser user);

        Task<bool> RemoveUserFromGroup(string groupid, string userId);

        Task<List<NotificationUser>> GetUsersFromGroup(string groupid);

        //Notifications
        Task<bool> AddNotifications(string keyId, Notification notification);

        
        Task<List<Notification>> GetNotificationAll(string keyId);
        Task<List<Notification>> GetNotificationAllByChannel(string keyId, string channel);

        Task ClearNotification(string keyId);

        Task<bool> AddProductUpdateMessage(string keyId, ProductUpdateMessage productUpdateMessage);
        Task<bool> AddStockTakeAdjustMessage(StockTakeAdjustItemInfo stockTakeAdjustItemInfo);
        
        Task<List<StockTakeAdjustItemInfo>> GetStockTakeAdjustMessage();
        Task<bool> DeleteStockTakeAdjustMessage();
        
        Task<bool> RemoveProductUpdateMessage(string keyId, string productVariantId);

        Task<List<ProductUpdateMessage>> GetProductUpdateMessage(string keyId);

    }
}