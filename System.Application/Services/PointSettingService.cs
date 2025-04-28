using System.Infrastructure.Repositories;
using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class PointSettingService : IPointSettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PointSettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PointSetting> GetPointSettingByIdAsync(int id)
        {
            var pointSetting = await _unitOfWork.GetRepository<PointSetting, int>().GetByIdAsync(id);
            if (pointSetting == null)
                throw new Exception("PointSetting not found.");
            return pointSetting;
        }

        public async Task<IEnumerable<PointSetting>> GetPointSettingsByStoreAsync(int storeId, bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<PointSetting, int>().FindAsync(ps => ps.StoreId == storeId, includeDeleted);
        }

        public async Task AddPointSettingAsync(PointSetting pointSetting)
        {
            await _unitOfWork.GetRepository<PointSetting, int>().AddAsync(pointSetting);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePointSettingAsync(PointSetting pointSetting)
        {
            var existingPointSetting = await GetPointSettingByIdAsync(pointSetting.Id);
            existingPointSetting.StoreId = pointSetting.StoreId;
            existingPointSetting.Amount = pointSetting.Amount;
            existingPointSetting.Points = pointSetting.Points;
            _unitOfWork.GetRepository<PointSetting, int>().Update(existingPointSetting);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePointSettingAsync(int id)
        {
            var pointSetting = await GetPointSettingByIdAsync(id);
            _unitOfWork.GetRepository<PointSetting, int>().Delete(pointSetting);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestorePointSettingAsync(int id)
        {
            await _unitOfWork.GetRepository<PointSetting, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}