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

        #region Notifications

        //* Send Order Notification
        public async Task<ApiResponse<bool>> SendOrderNotificationAsync(int storeId, int roomId)
        {
         
            var message = $"طلب جديد من الغرفة رقم {roomId}";
            await _hubContext.Clients.Group(storeId.ToString()).SendAsync("ReceiveNotification", message);
            return new ApiResponse<bool>(true, "تم إرسال إشعار الطلب بنجاح");
        }

        //* Send Assistance Request Notification
        public async Task<ApiResponse<bool>> SendAssistanceRequestNotificationAsync(int storeId, int roomId)
        {
           
            var message = $"طلب مساعدة جديد من الغرفة رقم {roomId}";
            await _hubContext.Clients.Group(storeId.ToString()).SendAsync("ReceiveNotification", message);
            return new ApiResponse<bool>(true, "تم إرسال إشعار طلب المساعدة بنجاح");
        }

        //* Send Gift Redemption Notification
        public async Task<ApiResponse<bool>> SendGiftRedemptionNotificationAsync(int storeId, int roomId)
        {

            var message = $"طلب استبدال هدية جديد من الغرفة رقم {roomId}";
            await _hubContext.Clients.Group(storeId.ToString()).SendAsync("ReceiveNotification", message);
            return new ApiResponse<bool>(true, "تم إرسال إشعار طلب استبدال الهدية بنجاح");
        }

        //* Send Order Status Update
        public async Task<ApiResponse<bool>> SendOrderStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null!)
        {
            var message = isApproved
                ? "تم الموافقة على طلبك"
                : $"تم رفض طلبك للسبب: {rejectionReason}";
            await _hubContext.Clients.Group($"Room_{roomId}")
                                     .SendAsync("ReceiveStatusUpdate", new { roomId = roomId.ToString(), message });

            return new ApiResponse<bool>(true, isApproved ? "تم إرسال إشعار الموافقة بنجاح" : "تم إرسال إشعار الرفض بنجاح");
        }

        //* Send Assistance Request Status Update
        public async Task<ApiResponse<bool>> SendAssistanceRequestStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null!)
        {
            var message = isApproved
                ? "تم الموافقة على طلب المساعدة الخاص بك"
                : $"تم رفض طلب المساعدة الخاص بك للسبب: {rejectionReason}";
            await _hubContext.Clients.Group($"Room_{roomId}")
                                     .SendAsync("ReceiveStatusUpdate", new { roomId = roomId.ToString(), message });
            return new ApiResponse<bool>(true, isApproved ? "تم إرسال إشعار الموافقة بنجاح" : "تم إرسال إشعار الرفض بنجاح");
        }

        //* Send Gift Redemption Status Update
        public async Task<ApiResponse<bool>> SendGiftRedemptionStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null!)
        {
            var message = isApproved
                ? "تم الموافقة على طلب استبدال الهدية الخاص بك"
                : $"تم رفض طلب استبدال الهدية الخاص بك للسبب: {rejectionReason}";
            await _hubContext.Clients.Group($"Room_{roomId}").SendAsync("ReceiveStatusUpdate", message);
            return new ApiResponse<bool>(true, isApproved ? "تم إرسال إشعار الموافقة بنجاح" : "تم إرسال إشعار الرفض بنجاح");
        }

        #endregion
    }
}