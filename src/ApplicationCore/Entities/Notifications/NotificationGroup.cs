using System.Collections;
using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities.Notifications
{
    public class NotificationUserGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<NotificationUser> NotificationUsers { get; set; } = new List<NotificationUser>();
    }
}