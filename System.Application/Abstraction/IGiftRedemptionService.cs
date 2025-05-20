using System.Shared;
using System.Shared.DTOs.GiftRedemptions;

namespace System.Application.Abstraction
{
    public interface IGiftRedemptionService
    {
        Task<ApiResponse<GiftRedemptionDto>> RequestGiftRedemptionAsync(CreateGiftRedemptionDto dto);
        Task<ApiResponse<GiftRedemptionDto>> UpdateRedemptionStatusAsync(int redemptionId, UpdateGiftRedemptionStatusDto dto);
        Task<ApiResponse<IEnumerable<GiftRedemptionDto>>> GetPendingRedemptionsAsync(int storeId);
        Task<ApiResponse<IEnumerable<GiftRedemptionDto>>> GetAllRedemptionsAsync(int storeId);
        Task<ApiResponse<IEnumerable<GiftRedemptionDto>>> GetCustomerRedemptionsAsync(int customerId);
        Task<ApiResponse<GiftRedemptionDto>> GetRedemptionDetailsAsync(int redemptionId);
    }
}
