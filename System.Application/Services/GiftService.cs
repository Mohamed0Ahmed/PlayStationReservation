using Mapster;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;
using System.Shared.DTOs.Gifts;

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
        public async Task<ApiResponse<GiftDto>> CreateGiftAsync(string name, int pointsRequired, int storeId)
        {
            var existingGift = await _unitOfWork.GetRepository<Gift, int>().FindAsync(
                g => g.Name == name && g.StoreId == storeId);
            if (existingGift.Any())
                return new ApiResponse<GiftDto>("الهدية موجودة بالفعل", 200);


            var gift = new Gift
            {
                Name = name,
                PointsRequired = pointsRequired,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Gift, int>().AddAsync(gift);
            await _unitOfWork.SaveChangesAsync();

            var giftDto = gift.Adapt<GiftDto>();
            return new ApiResponse<GiftDto>(giftDto, "تم إضافة الهدية بنجاح", 201);
        }


        //* Update Gift
        public async Task<ApiResponse<GiftDto>> UpdateGiftAsync(int giftId, string name, int pointsRequired)
        {
            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(giftId);
            if (gift == null)
                return new ApiResponse<GiftDto>("الهدية غير موجودة", 200);


            var existingGift = await _unitOfWork.GetRepository<Gift, int>().FindAsync(
                g => g.Name == name && g.StoreId == gift.StoreId && g.Id != giftId);
            if (existingGift.Any())
                return new ApiResponse<GiftDto>("اسم الهدية موجود بالفعل", 200);


            gift.Name = name;
            gift.PointsRequired = pointsRequired;
            gift.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Gift, int>().Update(gift);
            await _unitOfWork.SaveChangesAsync();

            var giftDto = gift.Adapt<GiftDto>();
            return new ApiResponse<GiftDto>(giftDto, "تم تعديل الهدية بنجاح");
        }


        //* Delete Gift
        public async Task<ApiResponse<bool>> DeleteGiftAsync(int giftId)
        {

            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(giftId);
            if (gift == null)
                return new ApiResponse<bool>("الهدية غير موجودة", 200);


            _unitOfWork.GetRepository<Gift, int>().Delete(gift);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف الهدية بنجاح");
        }


        //* Restore Gift
        public async Task<ApiResponse<bool>> RestoreGiftAsync(int giftId)
        {

            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(giftId, onlyDeleted: true);
            if (gift == null || !gift.IsDeleted)
                return new ApiResponse<bool>("الهدية غير موجودة أو غير محذوفة", 200);


            await _unitOfWork.GetRepository<Gift, int>().RestoreAsync(giftId);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع الهدية بنجاح");
        }


        //* Get Gifts
        public async Task<ApiResponse<IEnumerable<GiftDto>>> GetGiftsAsync(int storeId)
        {

            var gifts = await _unitOfWork.GetRepository<Gift, int>().FindAsync(g => g.StoreId == storeId);

            if (!gifts.Any())
                return new ApiResponse<IEnumerable<GiftDto>>("لا يوجد هدايا", 200);

            var giftsDto = gifts.Adapt<IEnumerable<GiftDto>>();
            return new ApiResponse<IEnumerable<GiftDto>>(giftsDto, "تم جلب الهدايا بنجاح");
        }

        //* Get Deleted Gifts
        public async Task<ApiResponse<IEnumerable<GiftDto>>> GetDeletedGiftsAsync(int storeId)
        {

            var gifts = await _unitOfWork.GetRepository<Gift, int>().FindAsync(g => g.StoreId == storeId , onlyDeleted:true);

            if (!gifts.Any())
                return new ApiResponse<IEnumerable<GiftDto>>("لا يوجد هدايا", 200);

            var giftsDto = gifts.Adapt<IEnumerable<GiftDto>>();
            return new ApiResponse<IEnumerable<GiftDto>>(giftsDto, "تم جلب الهدايا بنجاح");
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