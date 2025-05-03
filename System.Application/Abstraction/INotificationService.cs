using System.Shared;

namespace System.Application.Abstraction
{
    public interface INotificationService
    {
        Task<ApiResponse<bool>> SendOrderNotificationAsync(int storeId, int roomId);
        Task<ApiResponse<bool>> SendAssistanceRequestNotificationAsync(int storeId, int roomId);
        Task<ApiResponse<bool>> SendGiftRedemptionNotificationAsync(int storeId, int roomId);
        Task<ApiResponse<bool>> SendOrderStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null);
        Task<ApiResponse<bool>> SendAssistanceRequestStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null);
        Task<ApiResponse<bool>> SendGiftRedemptionStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null);
    }
}