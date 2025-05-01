using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IStoreService
    {
        Task<ApiResponse<Store>> CreateStoreAsync(string name, string ownerEmail);
        Task<ApiResponse<Store>> UpdateStoreAsync(int storeId, string name, string ownerEmail);
        Task<ApiResponse<bool>> DeleteStoreAsync(int storeId);
        Task<ApiResponse<List<Store>>> GetStoresAsync();
        Task<ApiResponse<Room>> CreateRoomAsync(int storeId, string username, string password);
        Task<ApiResponse<Room>> UpdateRoomAsync(int roomId, string username, string password);
        Task<ApiResponse<bool>> DeleteRoomAsync(int roomId);
        Task<ApiResponse<List<Room>>> GetRoomsAsync(int storeId);
        Task<ApiResponse<PointSetting>> CreatePointSettingAsync(int storeId, decimal amountPerPoint, int points);
        Task<ApiResponse<PointSetting>> UpdatePointSettingAsync(int settingId, decimal amountPerPoint, int points);
        Task<ApiResponse<bool>> DeletePointSettingAsync(int settingId);
        Task<ApiResponse<List<PointSetting>>> GetPointSettingsAsync(int storeId);
        Task<ApiResponse<int>> GetTotalStoresCountAsync();
    }
}