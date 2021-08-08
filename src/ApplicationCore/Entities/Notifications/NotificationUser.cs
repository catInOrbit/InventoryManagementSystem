using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.Notifications
{
    public class NotificationUser
    {
        public NotificationUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public NotificationUser(ApplicationUser applicationUser, string role)
        {
            Id = Guid.NewGuid().ToString();
            UserId = applicationUser.Id;
            UserName = applicationUser.Fullname;
            Role = role;
            Group = role;
        }
        
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
    }
}