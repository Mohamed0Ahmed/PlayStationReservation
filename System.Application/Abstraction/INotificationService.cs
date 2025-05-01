namespace System.Application.Abstraction
{
    public interface INotificationService
    {
        Task SendOrderNotificationAsync(int storeId, int roomId);
        Task SendAssistanceRequestNotificationAsync(int storeId, int roomId);
        Task SendGiftRedemptionNotificationAsync(int storeId, int roomId);
        Task SendOrderStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null);
        Task SendAssistanceRequestStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null);
        Task SendGiftRedemptionStatusUpdateAsync(int roomId, bool isApproved, string rejectionReason = null);
    }
}