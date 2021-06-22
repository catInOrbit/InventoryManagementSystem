using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Notifications;

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

    }
}