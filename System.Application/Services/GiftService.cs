using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class GiftService : IGiftService
    {
        private readonly IRepository<Gift, int> _giftRepository;
        private readonly IRepository<GiftRedemption, int> _giftRedemptionRepository;

        public GiftService(
            IRepository<Gift, int> giftRepository,
            IRepository<GiftRedemption, int> giftRedemptionRepository)
        {
            _giftRepository = giftRepository;
            _giftRedemptionRepository = giftRedemptionRepository;
        }

        public async Task<ApiResponse<Gift>> CreateGiftAsync(string name, int pointsRequired, int storeId)
        {
            var gift = new Gift
            {
                Name = name,
                PointsRequired = pointsRequired,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _giftRepository.AddAsync(gift);
            return new ApiResponse<Gift>(gift, "تم إضافة الهدية بنجاح", 201);
        }

        public async Task<ApiResponse<Gift>> UpdateGiftAsync(int giftId, string name, int pointsRequired)
        {
            var gift = await _giftRepository.GetByIdAsync(giftId);
            if (gift == null)
            {
                return new ApiResponse<Gift>("الهدية غير موجودة", 404);
            }

            gift.Name = name;
            gift.PointsRequired = pointsRequired;
            gift.LastModifiedOn = DateTime.UtcNow;
            _giftRepository.Update(gift);

            return new ApiResponse<Gift>(gift, "تم تعديل الهدية بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteGiftAsync(int giftId)
        {
            var gift = await _giftRepository.GetByIdAsync(giftId);
            if (gift == null)
            {
                return new ApiResponse<bool>("الهدية غير موجودة", 404);
            }

            _giftRepository.Delete(gift);
            return new ApiResponse<bool>(true, "تم حذف الهدية بنجاح");
        }

        public async Task<ApiResponse<List<Gift>>> GetGiftsAsync(int storeId)
        {
            var gifts = await _giftRepository.FindAsync(g => g.StoreId == storeId);
            return new ApiResponse<List<Gift>>(gifts.ToList());
        }

        public async Task<ApiResponse<int>> GetTotalGiftsCountAsync(int storeId)
        {
            var count = (await _giftRepository.FindAsync(g => g.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد الهدايا بنجاح");
        }

        public async Task<ApiResponse<int>> GetMostRequestedGiftCountAsync(int storeId)
        {
            var mostRequestedGift = (await _giftRedemptionRepository.FindWithIncludesAsync(
                gr => gr.Gift.StoreId == storeId,
                include: q => q.Include(gr => gr.Gift)))
                .GroupBy(gr => gr.GiftId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            var count = mostRequestedGift?.Count() ?? 0;
            return new ApiResponse<int>(count, "تم جلب عدد طلبات أكثر هدية بنجاح");
        }
    }
}