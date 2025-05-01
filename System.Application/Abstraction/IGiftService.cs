using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IGiftService
    {
        Task<ApiResponse<Gift>> CreateGiftAsync(string name, int pointsRequired, int storeId);
        Task<ApiResponse<Gift>> UpdateGiftAsync(int giftId, string name, int pointsRequired);
        Task<ApiResponse<bool>> DeleteGiftAsync(int giftId);
        Task<ApiResponse<List<Gift>>> GetGiftsAsync(int storeId);
        Task<ApiResponse<int>> GetTotalGiftsCountAsync(int storeId);
        Task<ApiResponse<int>> GetMostRequestedGiftCountAsync(int storeId);
    }
}