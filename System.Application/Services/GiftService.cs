using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;

namespace System.Application.Services
{
    public class GiftService : IGiftService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GiftService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Gifts

        //* Create Gift
        public async Task<ApiResponse<Gift>> CreateGiftAsync(string name, int pointsRequired, int storeId)
        {
            if (string.IsNullOrEmpty(name) || pointsRequired <= 0 || storeId <= 0)
                return new ApiResponse<Gift>("ادخل البيانات الصحيحة", 400);


            var existingGift = await _unitOfWork.GetRepository<Gift, int>().FindAsync(
                g => g.Name == name && g.StoreId == storeId);
            if (existingGift != null)
                return new ApiResponse<Gift>("الهدية موجودة بالفعل", 400);


            var gift = new Gift
            {
                Name = name,
                PointsRequired = pointsRequired,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Gift, int>().AddAsync(gift);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<Gift>(gift, "تم إضافة الهدية بنجاح", 201);
        }

        //* Update Gift
        public async Task<ApiResponse<Gift>> UpdateGiftAsync(int giftId, string name, int pointsRequired)
        {
            if (giftId <= 0 || string.IsNullOrEmpty(name) || pointsRequired <= 0)
                return new ApiResponse<Gift>("ادخل البيانات الصحيحة", 400);


            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(giftId);
            if (gift == null)
                return new ApiResponse<Gift>("الهدية غير موجودة", 404);


            var existingGift = await _unitOfWork.GetRepository<Gift, int>().FindAsync(
                g => g.Name == name && g.StoreId == gift.StoreId && g.Id != giftId);
            if (existingGift != null)
                return new ApiResponse<Gift>("اسم الهدية موجود بالفعل", 400);


            gift.Name = name;
            gift.PointsRequired = pointsRequired;
            gift.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Gift, int>().Update(gift);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<Gift>(gift, "تم تعديل الهدية بنجاح");
        }

        //* Delete Gift
        public async Task<ApiResponse<bool>> DeleteGiftAsync(int giftId)
        {

            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(giftId);
            if (gift == null)
                return new ApiResponse<bool>("الهدية غير موجودة", 404);


            _unitOfWork.GetRepository<Gift, int>().Delete(gift);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف الهدية بنجاح");
        }

        //* Restore Gift
        public async Task<ApiResponse<bool>> RestoreGiftAsync(int giftId)
        {

            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(giftId, onlyDeleted: true);
            if (gift == null || !gift.IsDeleted)
                return new ApiResponse<bool>("الهدية غير موجودة أو غير محذوفة", 404);


            await _unitOfWork.GetRepository<Gift, int>().RestoreAsync(giftId);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع الهدية بنجاح");
        }

        //* Get Gifts
        public async Task<ApiResponse<List<Gift>>> GetGiftsAsync(int storeId)
        {

            var gifts = await _unitOfWork.GetRepository<Gift, int>().FindAsync(g => g.StoreId == storeId);

            if (!gifts.Any())
                return new ApiResponse<List<Gift>>("لا يوجد هدايا", 404);


            return new ApiResponse<List<Gift>>(gifts.ToList(), "تم جلب الهدايا بنجاح");
        }

        //* Get Total Gifts Count
        public async Task<ApiResponse<int>> GetTotalGiftsCountAsync(int storeId)
        {

            var count = (await _unitOfWork.GetRepository<Gift, int>().FindAsync(g => g.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد الهدايا بنجاح");
        }

   

        #endregion
    }
}