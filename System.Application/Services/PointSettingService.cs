using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class PointSettingService : IPointSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreService _storeService;

        public PointSettingService(IUnitOfWork unitOfWork, IStoreService storeService)
        {
            _unitOfWork = unitOfWork;
            _storeService = storeService;
        }

        public async Task<PointSetting> GetPointSettingByIdAsync(int id)
        {
            var pointSetting = await _unitOfWork.GetRepository<PointSetting, int>().GetByIdAsync(id);
            if (pointSetting == null)
                throw new CustomException("PointSetting not found.", 404);
            return pointSetting;
        }

        public async Task<IEnumerable<PointSetting>> GetPointSettingsByStoreAsync(int storeId, bool includeDeleted = false)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            return await _unitOfWork.GetRepository<PointSetting, int>().FindAsync(ps => ps.StoreId == storeId, includeDeleted);
        }

        public async Task AddPointSettingAsync(PointSetting pointSetting)
        {
            if (pointSetting.Amount < 0)
                throw new CustomException("Amount cannot be negative.", 400);
            if (pointSetting.Points < 0)
                throw new CustomException("Points cannot be negative.", 400);

            // Check for duplicate Amount and StoreId
            var existingSetting = (await _unitOfWork.GetRepository<PointSetting, int>().FindAsync(ps => ps.Amount == pointSetting.Amount && ps.StoreId == pointSetting.StoreId && !ps.IsDeleted)).FirstOrDefault();
            if (existingSetting != null)
                throw new CustomException($"A point setting with the amount '{pointSetting.Amount}' already exists for this store.", 400);

            await _storeService.GetStoreByIdAsync(pointSetting.StoreId);
            await _unitOfWork.GetRepository<PointSetting, int>().AddAsync(pointSetting);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePointSettingAsync(PointSetting pointSetting)
        {
            var existingPointSetting = await GetPointSettingByIdAsync(pointSetting.Id);
            if (pointSetting.Amount < 0)
                throw new CustomException("Amount cannot be negative.", 400);
            if (pointSetting.Points < 0)
                throw new CustomException("Points cannot be negative.", 400);

            // Check for duplicate Amount and StoreId (excluding the current setting)
            var duplicateSetting = (await _unitOfWork.GetRepository<PointSetting, int>().FindAsync(ps => ps.Amount == pointSetting.Amount && ps.StoreId == pointSetting.StoreId && ps.Id != pointSetting.Id && !ps.IsDeleted)).FirstOrDefault();
            if (duplicateSetting != null)
                throw new CustomException($"Another point setting with the amount '{pointSetting.Amount}' already exists for this store.", 400);

            await _storeService.GetStoreByIdAsync(pointSetting.StoreId);
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
            var pointSetting = await _unitOfWork.GetRepository<PointSetting, int>().GetByIdAsync(id, true);
            if (pointSetting == null)
                throw new CustomException("PointSetting not found.", 404);
            if (!pointSetting.IsDeleted)
                throw new CustomException("PointSetting is not deleted.", 400);

            await _unitOfWork.GetRepository<PointSetting, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}