using System.Shared;
using System.Shared.DTOs.Gifts;

namespace System.Application.Abstraction
{
    public interface IGiftService
    {
        Task<ApiResponse<GiftDto>> CreateGiftAsync(string name, int pointsRequired, int storeId);
        Task<ApiResponse<GiftDto>> UpdateGiftAsync(int giftId, string name, int pointsRequired);
        Task<ApiResponse<bool>> DeleteGiftAsync(int giftId);
        Task<ApiResponse<bool>> RestoreGiftAsync(int giftId);
        Task<ApiResponse<IEnumerable<GiftDto>>> GetGiftsAsync(int storeId);
        Task<ApiResponse<IEnumerable<GiftDto>>> GetDeletedGiftsAsync(int storeId);
        Task<ApiResponse<int>> GetTotalGiftsCountAsync(int storeId);
    }
}