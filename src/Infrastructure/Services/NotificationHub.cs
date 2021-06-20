using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class NotificationHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("NotificationMessage", "Connected");

        }
        
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("NotificationMessage", user, message);
        }
        
        public async Task SendMessageToGroup(string groupName, string message)
        {
            await Clients.Groups(groupName).SendAsync("NotificationGroupMessage", message);
        }
        
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}