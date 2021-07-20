using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class NotificationHub : Hub
    {
        private IUserSession _userSession;

        public NotificationHub(IUserSession userSession)
        {
            _userSession = userSession;
        }

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
        
        public async Task ConnectWithResource(string userId, string resourceId)
        {
            if(await _userSession.SaveUserResourceAccess(userId, resourceId))
                await Clients.Caller.SendAsync("ResourceChecker", "True, Using resource: " + resourceId);
            else
                await Clients.Caller.SendAsync("ResourceChecker", "False, Page already being used");
        }
        
        public async Task DisconnectFromResource(string userId, string resourceId)
        {
            if(await _userSession.RemoveUserFromResource(userId, resourceId))
                await Clients.Caller.SendAsync("ResourceChecker", "True, Remove from resource: " + resourceId);
            
            else
                await Clients.Caller.SendAsync("ResourceChecker", "False, Invalid remove resource operation: " + resourceId);
        }
    }
}