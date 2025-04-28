using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IPointSettingService
    {
        Task<PointSetting> GetPointSettingByIdAsync(int id);
        Task<IEnumerable<PointSetting>> GetPointSettingsByStoreAsync(int storeId, bool includeDeleted = false);
        Task AddPointSettingAsync(PointSetting pointSetting);
        Task UpdatePointSettingAsync(PointSetting pointSetting);
        Task DeletePointSettingAsync(int id);
        Task RestorePointSettingAsync(int id);
    }
}