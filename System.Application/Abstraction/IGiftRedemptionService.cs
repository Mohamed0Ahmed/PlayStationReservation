using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IGiftRedemptionService
    {
        Task<ApiResponse<GiftRedemption>> CreateGiftRedemptionAsync(string phoneNumber, int giftId, int roomId);
        Task<ApiResponse<List<GiftRedemption>>> GetPendingGiftRedemptionsAsync(int storeId);
        Task<ApiResponse<List<GiftRedemption>>> GetGiftRedemptionsAsync(int storeId, bool includeDeleted = false);
        Task<ApiResponse<GiftRedemption>> ApproveGiftRedemptionAsync(int redemptionId);
        Task<ApiResponse<GiftRedemption>> RejectGiftRedemptionAsync(int redemptionId, string rejectionReason);
        Task<ApiResponse<int>> GetTotalGiftRedemptionsCountAsync(int storeId);
        Task<ApiResponse<int>> GetTotalPointsRedeemedAsync(int storeId);
    }
}