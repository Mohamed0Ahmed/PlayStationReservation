using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class GiftRedemptionService : IGiftRedemptionService
    {
        private readonly IRepository<GiftRedemption, int> _giftRedemptionRepository;
        private readonly IRepository<Gift, int> _giftRepository;
        private readonly IRepository<Customer, int> _customerRepository;
        private readonly INotificationService _notificationService;

        public GiftRedemptionService(
            IRepository<GiftRedemption, int> giftRedemptionRepository,
            IRepository<Gift, int> giftRepository,
            IRepository<Customer, int> customerRepository,
            INotificationService notificationService)
        {
            _giftRedemptionRepository = giftRedemptionRepository;
            _giftRepository = giftRepository;
            _customerRepository = customerRepository;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<GiftRedemption>> CreateGiftRedemptionAsync(int customerId, int giftId, int roomId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiResponse<GiftRedemption>("الزبون غير موجود", 404);
            }

            var gift = await _giftRepository.GetByIdAsync(giftId);
            if (gift == null)
            {
                return new ApiResponse<GiftRedemption>("الهدية غير موجودة", 404);
            }

            if (customer.Points < gift.PointsRequired)
            {
                return new ApiResponse<GiftRedemption>("النقاط غير كافية", 400);
            }

            var redemption = new GiftRedemption
            {
                CustomerId = customerId,
                GiftId = giftId,
                RedemptionDate = DateTime.UtcNow,
                Status = "Pending",
                CreatedOn = DateTime.UtcNow
            };

            await _giftRedemptionRepository.AddAsync(redemption);
            await _notificationService.SendGiftRedemptionNotificationAsync(gift.StoreId, roomId);
            return new ApiResponse<GiftRedemption>(redemption, "تم إضافة طلب استبدال الهدية بنجاح", 201);
        }

        public async Task<ApiResponse<List<GiftRedemption>>> GetPendingGiftRedemptionsAsync(int storeId)
        {
            var redemptions = await _giftRedemptionRepository.FindWithIncludesAsync(
                predicate: gr => gr.Gift.StoreId == storeId && gr.Status == "Pending",
                include: q => q.Include(gr => gr.Customer).Include(gr => gr.Gift),
                includeDeleted: false);

            return new ApiResponse<List<GiftRedemption>>(redemptions.ToList());
        }

        public async Task<ApiResponse<List<GiftRedemption>>> GetGiftRedemptionsAsync(int storeId, bool includeDeleted = false)
        {
            var redemptions = await _giftRedemptionRepository.FindWithIncludesAsync(
                predicate: gr => gr.Gift.StoreId == storeId,
                include: q => q.Include(gr => gr.Customer).Include(gr => gr.Gift),
                includeDeleted: includeDeleted);

            return new ApiResponse<List<GiftRedemption>>(redemptions.ToList());
        }

        public async Task<ApiResponse<GiftRedemption>> ApproveGiftRedemptionAsync(int redemptionId)
        {
            var redemption = await _giftRedemptionRepository.GetByIdWithIncludesAsync(
                redemptionId,
                include: q => q.Include(gr => gr.Customer).Include(gr => gr.Gift).Include(gr => gr.Customer));
            if (redemption == null)
            {
                return new ApiResponse<GiftRedemption>("طلب الاستبدال غير موجود", 404);
            }

            redemption.Status = "Accepted";
            redemption.LastModifiedOn = DateTime.UtcNow;
            redemption.Customer.Points -= redemption.Gift.PointsRequired;
            _customerRepository.Update(redemption.Customer);
            _giftRedemptionRepository.Update(redemption);

            await _notificationService.SendGiftRedemptionStatusUpdateAsync(redemption.Customer.Id, true);
            return new ApiResponse<GiftRedemption>(redemption, "تم الموافقة على طلب الاستبدال بنجاح");
        }

        public async Task<ApiResponse<GiftRedemption>> RejectGiftRedemptionAsync(int redemptionId, string rejectionReason)
        {
            var redemption = await _giftRedemptionRepository.GetByIdWithIncludesAsync(
                redemptionId,
                include: q => q.Include(gr => gr.Customer));
            if (redemption == null)
            {
                return new ApiResponse<GiftRedemption>("طلب الاستبدال غير موجود", 404);
            }

            redemption.Status = "Rejected";
            redemption.RejectionReason = rejectionReason;
            redemption.LastModifiedOn = DateTime.UtcNow;
            _giftRedemptionRepository.Update(redemption);

            await _notificationService.SendGiftRedemptionStatusUpdateAsync(redemption.Customer.Id, false, rejectionReason);
            return new ApiResponse<GiftRedemption>(redemption, "تم رفض طلب الاستبدال بنجاح");
        }

        public async Task<ApiResponse<int>> GetTotalGiftRedemptionsCountAsync(int storeId)
        {
            var count = (await _giftRedemptionRepository.FindAsync(gr => gr.Gift.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد طلبات الاستبدال بنجاح");
        }

        public async Task<ApiResponse<int>> GetTotalPointsRedeemedAsync(int storeId)
        {
            var redemptions = await _giftRedemptionRepository.FindWithIncludesAsync(
                gr => gr.Gift.StoreId == storeId && gr.Status == "Accepted",
                include: q => q.Include(gr => gr.Gift));
            var totalPoints = redemptions.Sum(gr => gr.Gift.PointsRequired);
            return new ApiResponse<int>(totalPoints, "تم جلب إجمالي النقاط المستبدلة بنجاح");
        }
    }
}