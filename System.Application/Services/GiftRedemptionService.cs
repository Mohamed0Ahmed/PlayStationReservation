using Mapster;
using System.Application.Abstraction;
using System.Domain.Enums;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;
using System.Shared.DTOs.GiftRedemptions;

namespace System.Application.Services
{
    public class GiftRedemptionService : IGiftRedemptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;

        public GiftRedemptionService(IUnitOfWork unitOfWork, ICustomerService customerService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<GiftRedemptionDto>> RequestGiftRedemptionAsync(CreateGiftRedemptionDto dto)
        {
            // Get the gift
            var gift = await _unitOfWork.GetRepository<Gift, int>().GetByIdAsync(dto.GiftId);
            if (gift == null)
                return new ApiResponse<GiftRedemptionDto>("الهدية غير موجودة", 200);

            // Get room
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(dto.RoomId);
            if (room == null || room.StoreId != gift.StoreId)
                return new ApiResponse<GiftRedemptionDto>("الغرفة غير موجودة أو غير صالحة", 200);

            // Get customer by phone number
            var customerResponse = await _customerService.GetCustomerByPhoneAsync(dto.CustomerNumber, room.StoreId);
            if (!customerResponse.IsSuccess)
                return new ApiResponse<GiftRedemptionDto>("العميل غير موجود", 200);

            var customer = customerResponse.Data;

            // Check if customer has enough points
            if (customer.Points < gift.PointsRequired)
                return new ApiResponse<GiftRedemptionDto>("نقاطك غير كافية لاستبدال هذه الهدية", 200);



            // Create redemption request
            var redemption = new GiftRedemption
            {
                CustomerNumber = customer.PhoneNumber,
                RoomName = room.Username,
                GiftName = gift.Name,
                GiftId = gift.Id,
                CustomerId = customer.Id,
                RoomId = room.Id,
                StoreId = gift.StoreId,
                PointsUsed = gift.PointsRequired,
                Status = Status.Pending,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<GiftRedemption, int>().AddAsync(redemption);
            await _unitOfWork.SaveChangesAsync();

            // Send notification to store owner
            await _notificationService.SendGiftRedemptionNotificationAsync(redemption.StoreId, redemption.RoomId);

            var result = redemption.Adapt<GiftRedemptionDto>();


            return new ApiResponse<GiftRedemptionDto>(result, "تم تقديم طلب استبدال النقاط بنجاح", 201);
        }


        public async Task<ApiResponse<GiftRedemptionDto>> UpdateRedemptionStatusAsync(int redemptionId, UpdateGiftRedemptionStatusDto dto)
        {
            var redemption = await _unitOfWork.GetRepository<GiftRedemption, int>().GetByIdAsync(redemptionId);
            if (redemption == null)
                return new ApiResponse<GiftRedemptionDto>("طلب الاستبدال غير موجود", 200);

            var customer = await _unitOfWork.GetRepository<Customer, int>().GetByIdAsync(redemption.CustomerId);
            if (customer == null)
                return new ApiResponse<GiftRedemptionDto>("هذا الكلاينت غير موجود", 200);

            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(redemption.RoomId);
            if (room == null)
                return new ApiResponse<GiftRedemptionDto>("هذه الغرفة غير موجودة", 200);


            if (redemption.Status != Status.Pending)
                return new ApiResponse<GiftRedemptionDto>("لا يمكن تعديل حالة الطلب بعد معالجته", 200);

            if (dto.IsApproved)
            {
                customer.Points -= redemption.PointsUsed;
                _unitOfWork.GetRepository<Customer, int>().Update(customer);

                redemption.Status = Status.Accepted;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                    return new ApiResponse<GiftRedemptionDto>("يجب إدخال سبب الرفض", 200);

                redemption.Status = Status.Rejected;
                redemption.RejectionReason = dto.RejectionReason;
            }

            redemption.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<GiftRedemption, int>().Update(redemption);
            await _unitOfWork.SaveChangesAsync();

            // Send status update notification to the room
            await _notificationService.SendGiftRedemptionStatusUpdateAsync(
                redemption.RoomId,
                dto.IsApproved,
                dto.IsApproved ? null : dto.RejectionReason);

            var result = redemption.Adapt<GiftRedemptionDto>();
            result.GiftName = room.Username;
            result.CustomerPhone = customer.PhoneNumber;

            return new ApiResponse<GiftRedemptionDto>(
                result,
                dto.IsApproved ? "تمت الموافقة على الطلب بنجاح" : "تم رفض الطلب");
        }


        public async Task<ApiResponse<IEnumerable<GiftRedemptionDto>>> GetPendingRedemptionsAsync(int storeId)
        {
            var redemptions = await _unitOfWork.GetRepository<GiftRedemption, int>()
                .FindWithIncludesAsync(
                    predicate: r => r.StoreId == storeId && r.Status == Status.Pending);

            var result = redemptions.Select(r => new GiftRedemptionDto
            {
                Id = r.Id,
                GiftName = r.GiftName,
                CustomerPhone = r.CustomerNumber,
                RoomName = r.RoomName,
                StoreId = r.StoreId,
                PointsUsed = r.PointsUsed,
                Status = r.Status.ToString(),
                RejectionReason = r.RejectionReason,
            });

            return new ApiResponse<IEnumerable<GiftRedemptionDto>>(result);
        }


        public async Task<ApiResponse<IEnumerable<GiftRedemptionDto>>> GetCustomerRedemptionsAsync(int customerId)
        {
            var redemptions = await _unitOfWork.GetRepository<GiftRedemption, int>()
                .FindAsync(r => r.CustomerId == customerId);

            var result = redemptions.Select(r => new GiftRedemptionDto
            {
                Id = r.Id,
                GiftName = r.GiftName,
                CustomerPhone = r.CustomerNumber,
                RoomName = r.RoomName,
                StoreId = r.StoreId,
                PointsUsed = r.PointsUsed,
                Status = r.Status.ToString(),
                RejectionReason = r.RejectionReason,
            });

            return new ApiResponse<IEnumerable<GiftRedemptionDto>>(result);
        }


        public async Task<ApiResponse<GiftRedemptionDto>> GetRedemptionDetailsAsync(int redemptionId)
        {

            var redemption = await _unitOfWork.GetRepository<GiftRedemption, int>().GetByIdAsync(redemptionId);
            if (redemption == null)
                return new ApiResponse<GiftRedemptionDto>("طلب الاستبدال غير موجود", 200);

            var result = redemption.Adapt<GiftRedemptionDto>();
            return new ApiResponse<GiftRedemptionDto>(result);
        }

        public async Task<ApiResponse<IEnumerable<GiftRedemptionDto>>> GetAllRedemptionsAsync(int storeId)
        {
            var redemptions = await _unitOfWork.GetRepository<GiftRedemption, int>()
                .FindAsync(r => r.StoreId == storeId);
            if (redemptions == null)
                return new ApiResponse<IEnumerable<GiftRedemptionDto>>("لا يوجد طلبات هدايا حاليا" , 200);

            var result = redemptions.Select(r => new GiftRedemptionDto
            {
                Id = r.Id,
                GiftName = r.GiftName,
                CustomerPhone = r.CustomerNumber,
                RoomName = r.RoomName,
                StoreId = r.StoreId,
                PointsUsed = r.PointsUsed,
                Status = r.Status.ToString(),
                RejectionReason = r.RejectionReason,
            });

            return new ApiResponse<IEnumerable<GiftRedemptionDto>>(result);
        }
    }
}
