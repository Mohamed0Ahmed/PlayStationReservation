using Microsoft.AspNetCore.SignalR;

namespace System.Shared
{
    public class NotificationHub : Hub
    {
        public async Task JoinStoreGroup(int storeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Store_{storeId}");
        }

        public async Task LeaveStoreGroup(int storeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Store_{storeId}");
        }
    }
}