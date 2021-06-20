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
}