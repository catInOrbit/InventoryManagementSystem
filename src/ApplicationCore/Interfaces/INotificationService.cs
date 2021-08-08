using System.Threading.Tasks;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface INotificationService
    {
        // public Task SendNotification(string userId, string message);
        public Task SendNotificationGroup(string groupName, string userId, string message, string type, string typeId);
        
        public string CreateMessage(string fromUserFullname, string action, string page, string objectId);
        
    }
}