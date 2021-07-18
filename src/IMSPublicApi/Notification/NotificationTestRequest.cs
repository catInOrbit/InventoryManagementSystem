using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi
{
    public class NotificationTestRequest
    {
        public string UserId { get; set; }
        public string Message { get; set; }
    }
    
    public class NotificationGroupTestRequest
    {
        public string UserId { get; set; }
        public string Group { get; set; }
        public string Message { get; set; }
    }
    
    public class NotificationGetRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
    
    public class NotificationGetResponse : BaseSearchResponse<Notification>
    {
    }
}