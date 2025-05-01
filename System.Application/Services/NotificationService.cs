using Microsoft.AspNetCore.SignalR;
using System.Application.Abstraction;
using System.Shared;

namespace System.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendOrderNotificationAsync(int storeId, int roomId)
        {
            var message = $"طلب جديد من الغرفة رقم {roomId}";
            await _hubContext.Clients.Group(storeId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task SendAssistanceRequestNotificationAsync(int storeId, int roomId)
        {
            var message = $"طلب مساعدة جديد من الغرفة رقم {roomId}";
            await _hubContext.Clients.Group(storeId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task SendGiftRedemptionNotificationAsync(int storeId, int roomId)
        {
            var message = $"طلب استبدال هدية جديد من الغرفة رقم {roomId}";
            await _hubContext.Clients.Group(storeId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task SendOrderStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null)
        {
            var message = isApproved
                ? "تم الموافقة على طلبك"
                : $"تم رفض طلبك للسبب: {rejectionReason}";
            await _hubContext.Clients.Group($"Room_{roomId}").SendAsync("ReceiveStatusUpdate", message);
        }

        public async Task SendAssistanceRequestStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null)
        {
            var message = isApproved
                ? "تم الموافقة على طلب المساعدة الخاص بك"
                : $"تم رفض طلب المساعدة الخاص بك للسبب: {rejectionReason}";
            await _hubContext.Clients.Group($"Room_{roomId}").SendAsync("ReceiveStatusUpdate", message);
        }

        public async Task SendGiftRedemptionStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null)
        {
            var message = isApproved
                ? "تم الموافقة على طلب استبدال الهدية الخاص بك"
                : $"تم رفض طلب استبدال الهدية الخاص بك للسبب: {rejectionReason}";
            await _hubContext.Clients.Group($"Room_{roomId}").SendAsync("ReceiveStatusUpdate", message);
        }
    }
}