namespace InventoryManagementSystem.PublicApi
{
    public class CustomRedisConfiguration
    {
        public bool AllowAdmin { get; set; }
        public string EndPoint { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public int ConnectTimeout { get; set; }
        public int ConnectRetry { get; set; }
        public int SyncTimeout { get; set; }
        public bool AbortOnConnectFail { get; set; }
    }
}