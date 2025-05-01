using Microsoft.AspNetCore.SignalR;

namespace System.Shared
{
    public class NotificationHub : Hub
    {
        public async Task JoinStoreGroup(string storeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, storeId);
        }

        public async Task LeaveStoreGroup(string storeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, storeId);
        }

        public async Task JoinRoomGroup(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Room_{roomId}");
        }

        public async Task LeaveRoomGroup(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Room_{roomId}");
        }
    }
}