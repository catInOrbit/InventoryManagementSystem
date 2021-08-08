using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities.Notifications
{
    public class GroupNotifications
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}