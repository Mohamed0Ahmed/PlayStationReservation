using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;

namespace System.Application.Services
{
    public class GiftRedemptionService : IGiftRedemptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public GiftRedemptionService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        #region Gift Redemptions

        //* Create Gift Redemption
        public async Task<ApiResponse<GiftRedemption>> CreateGiftRedemptionAsync(string phoneNumber, int giftId, int roomId)
        {
            if (string.IsNullOrEmpty(phoneNumber) || giftId <= 0 || roomId <= 0)
                return new ApiResponse<GiftRedemption>("ادخل البيانات الصحيحة", 400);


            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);
            if (room == null)
                return new ApiResponse<GiftRedemption>("الغرفة غير موجودة", 404);


            var customers = await _unitOfWork.GetRepository<Customer, int>().FindWithIncludesAsync(
                c => c.PhoneNumber == phoneNumber && c.StoreId == room.StoreId,
                includeDeleted: false);
            var customer = customers.FirstOrDefault();

            if (customer == null)
                return new ApiResponse<GiftRedemption>("العميل غير مسجل، سجل برقم تليفونك أولاً", 400);


            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(giftId);
            if (gift == null)
                return new ApiResponse<GiftRedemption>("الهدية غير موجودة", 404);


            if (customer.Points < gift.PointsRequired)
                return new ApiResponse<GiftRedemption>("النقاط غير كافية", 400);


            var redemption = new GiftRedemption
            {
                CustomerId = customer.Id,
                GiftId = giftId,
                RedemptionDate = DateTime.UtcNow,
                Status = "Pending",
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<GiftRedemption, int>().AddAsync(redemption);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendGiftRedemptionNotificationAsync(gift.StoreId, roomId);
            return new ApiResponse<GiftRedemption>(redemption, "تم إضافة طلب استبدال الهدية بنجاح", 201);
        }

        //* Get Pending Gift Redemptions
        public async Task<ApiResponse<List<GiftRedemption>>> GetPendingGiftRedemptionsAsync(int storeId)
        {


            var redemptions = await _unitOfWork.GetRepository<GiftRedemption, int>().FindWithIncludesAsync(
                predicate: gr => gr.Gift.StoreId == storeId && gr.Status == "Pending",
                include: q => q.Include(gr => gr.Customer).Include(gr => gr.Gift),
                includeDeleted: false);

            if (!redemptions.Any())
            {
                return new ApiResponse<List<GiftRedemption>>("لا يوجد طلبات استبدال معلقة", 404);
            }

            return new ApiResponse<List<GiftRedemption>>(redemptions.ToList(), "تم جلب طلبات الاستبدال المعلقة بنجاح");
        }

        //* Get All Gift Redemptions
        public async Task<ApiResponse<List<GiftRedemption>>> GetGiftRedemptionsAsync(int storeId, bool includeDeleted = false)
        {


            var redemptions = await _unitOfWork.GetRepository<GiftRedemption, int>().FindWithIncludesAsync(
                predicate: gr => gr.Gift.StoreId == storeId,
                include: q => q.Include(gr => gr.Customer).Include(gr => gr.Gift),
                includeDeleted: includeDeleted);

            if (!redemptions.Any())
                return new ApiResponse<List<GiftRedemption>>("لا يوجد طلبات استبدال", 404);


            return new ApiResponse<List<GiftRedemption>>(redemptions.ToList(), "تم جلب طلبات الاستبدال بنجاح");
        }

        //* Approve Gift Redemption
        public async Task<ApiResponse<GiftRedemption>> ApproveGiftRedemptionAsync(int redemptionId)
        {
            if (redemptionId <= 0)
                return new ApiResponse<GiftRedemption>("معرف طلب الاستبدال غير صالح", 400);


            var redemption = await _unitOfWork.GetRepository<GiftRedemption, int>().GetByIdWithIncludesAsync(
                redemptionId, include: q => q.Include(gr => gr.Customer).Include(gr => gr.Gift));
            if (redemption == null)
                return new ApiResponse<GiftRedemption>("طلب الاستبدال غير موجود", 404);


            if (redemption.Status != "Pending")
                return new ApiResponse<GiftRedemption>("لا يمكن الموافقة على طلب غير معلق", 400);


            redemption.Status = "Accepted";
            redemption.LastModifiedOn = DateTime.UtcNow;
            redemption.Customer.Points -= redemption.Gift.PointsRequired;
            _unitOfWork.GetRepository<Customer, int>().Update(redemption.Customer);
            _unitOfWork.GetRepository<GiftRedemption, int>().Update(redemption);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendGiftRedemptionStatusUpdateAsync(redemption.CustomerId, true);
            return new ApiResponse<GiftRedemption>(redemption, "تم الموافقة على طلب الاستبدال بنجاح");
        }

        //* Reject Gift Redemption
        public async Task<ApiResponse<GiftRedemption>> RejectGiftRedemptionAsync(int redemptionId, string rejectionReason)
        {


            if (string.IsNullOrEmpty(rejectionReason))
                return new ApiResponse<GiftRedemption>("سبب الرفض مطلوب", 400);


            var redemption = await _unitOfWork.GetRepository<GiftRedemption, int>().GetByIdWithIncludesAsync(
                redemptionId, include: q => q.Include(gr => gr.Customer));
            if (redemption == null)
                return new ApiResponse<GiftRedemption>("طلب الاستبدال غير موجود", 404);


            if (redemption.Status != "Pending")
                return new ApiResponse<GiftRedemption>("لا يمكن رفض طلب غير معلق", 400);


            redemption.Status = "Rejected";
            redemption.RejectionReason = rejectionReason;
            redemption.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<GiftRedemption, int>().Update(redemption);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendGiftRedemptionStatusUpdateAsync(redemption.CustomerId, false, rejectionReason);
            return new ApiResponse<GiftRedemption>(redemption, "تم رفض طلب الاستبدال بنجاح");
        }

        //* Get Total Gift Redemptions Count
        public async Task<ApiResponse<int>> GetTotalGiftRedemptionsCountAsync(int storeId)
        {
            var count = (await _unitOfWork.GetRepository<GiftRedemption, int>().FindAsync(
                gr => gr.Gift.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد طلبات الاستبدال بنجاح");
        }

        //* Get Total Points Redeemed
        public async Task<ApiResponse<int>> GetTotalPointsRedeemedAsync(int storeId)
        {
            var redemptions = await _unitOfWork.GetRepository<GiftRedemption, int>().FindWithIncludesAsync(
                gr => gr.Gift.StoreId == storeId && gr.Status == "Accepted",
                include: q => q.Include(gr => gr.Gift));
            var totalPoints = redemptions.Sum(gr => gr.Gift.PointsRequired);
            return new ApiResponse<int>(totalPoints, "تم جلب إجمالي النقاط المستبدلة بنجاح");
        }

        #endregion
    }
}