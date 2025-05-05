using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs.Rooms;
using System.Shared.DTOs.Stores;

namespace System.Application.Abstraction
{
    public interface IStoreService
    {
        Task<ApiResponse<StoreDto>> CreateStoreAsync(string name, string ownerEmail);
        Task<ApiResponse<StoreDto>> UpdateStoreAsync(int storeId, string name, string ownerEmail);
        Task<ApiResponse<bool>> DeleteStoreAsync(int storeId);
        Task<ApiResponse<bool>> RestoreStoreAsync(int storeId);
        Task<ApiResponse<List<StoreDto>>> GetStoresAsync();
        Task<ApiResponse<List<StoreDto>>> GetDeletedStoresAsync();
        Task<ApiResponse<RoomDto>> CreateRoomAsync(int storeId, string username, string password);
        Task<ApiResponse<RoomDto>> UpdateRoomAsync(int roomId, string username, string password);
        Task<ApiResponse<bool>> DeleteRoomAsync(int roomId);
        Task<ApiResponse<bool>> RestoreRoomAsync(int roomId);
        Task<ApiResponse<List<RoomDto>>> GetRoomsAsync(int storeId);
        Task<ApiResponse<PointSetting>> CreatePointSettingAsync(int storeId, decimal amountPerPoint, int points );
        Task<ApiResponse<PointSetting>> UpdatePointSettingAsync(int settingId, decimal amountPerPoint, int points);
        Task<ApiResponse<bool>> DeletePointSettingAsync(int settingId);
        Task<ApiResponse<List<PointSetting>>> GetPointSettingsAsync(int storeId);
        Task<ApiResponse<int>> GetTotalStoresCountAsync();
    }
}