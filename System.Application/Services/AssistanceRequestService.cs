using Mapster;
using System.Application.Abstraction;
using System.Domain.Enums;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;
using System.Shared.DTOs.Requests;
using Request = System.Domain.Models.Request;


namespace System.Application.Services
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public RequestService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        #region Assistance Requests

        //* Create Assistance Request
        public async Task<ApiResponse<RequestDto>> CreateAssistanceRequestAsync(int roomId, int requestTypeId)
        {

            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);

            if (room == null)
                return new ApiResponse<RequestDto>("الغرفة غير موجودة", 200);


            // التحقق من نوع المساعدة (Custom أو Default)
            var customType = await _unitOfWork.GetRepository<AssistanceRequestType, int>().GetByIdAsync(requestTypeId);
            if (customType == null)
            {
                var defaultType = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetByIdAsync(requestTypeId);
                if (defaultType == null)
                    return new ApiResponse<RequestDto>("نوع المساعدة غير موجود", 200);
            }


            var request = new Request
            {
                RoomName = room.Username,
                StoreId = room.StoreId,
                RoomId = roomId,
                RequestTypeId = requestTypeId,
                Status = Status.Pending,
                RequestDate = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Request, int>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            var requestDto = request.Adapt<RequestDto>();
            await _notificationService.SendAssistanceRequestNotificationAsync(room.StoreId, roomId);
            return new ApiResponse<RequestDto>(requestDto, "تم إضافة طلب المساعدة بنجاح", 201);
        }

        //* Get Pending Assistance Requests
        public async Task<ApiResponse<IEnumerable<RequestDto>>> GetPendingAssistanceRequestsAsync(int storeId)
        {
            var requests = await _unitOfWork.GetRepository<Request, int>().FindAsync(
                predicate: ar => ar.StoreId == storeId && ar.Status == Status.Pending);

            if (!requests.Any())
                return new ApiResponse<IEnumerable<RequestDto>>("لا يوجد طلبات مساعدة حاليا", 200);

            var requestDto = requests.Adapt<IEnumerable<RequestDto>>();

            return new ApiResponse<IEnumerable<RequestDto>>(requestDto, "المساعدات المطلوبة حاليا");
        }

        //* Get All Assistance Requests
        public async Task<ApiResponse<IEnumerable<RequestDto>>> GetAssistanceRequestsAsync(int storeId, bool includeDeleted = false)
        {
            if (storeId <= 0)
                return new ApiResponse<IEnumerable<RequestDto>>("المحل ده مش موجود", 200);


            var requests = await _unitOfWork.GetRepository<Request, int>().FindWithIncludesAsync(
                predicate: ar => ar.StoreId == storeId);

            if (!requests.Any())
                return new ApiResponse<IEnumerable<RequestDto>>("لا يوجد طلبات مساعدة", 200);

            var requestDto = requests.Adapt<IEnumerable<RequestDto>>();
            return new ApiResponse<IEnumerable<RequestDto>>(requestDto, "تم جلب طلبات المساعدة بنجاح");
        }

        //* Approve Assistance Request
        public async Task<ApiResponse<RequestDto>> ApproveAssistanceRequestAsync(int requestId)
        {
            var request = await _unitOfWork.GetRepository<Request, int>().GetByIdWithIncludesAsync(requestId);

            if (request == null)
                return new ApiResponse<RequestDto>("طلب المساعدة غير موجود", 200);


            if (request.Status != Status.Pending)
                return new ApiResponse<RequestDto>("لا يمكن الموافقة على طلب غير معلق", 200);

            request.Status = Status.Accepted;
            request.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Request, int>().Update(request);
            await _unitOfWork.SaveChangesAsync();


            var requestDto = request.Adapt<RequestDto>();
            await _notificationService.SendAssistanceRequestStatusUpdateAsync(request.RoomId, true);
            return new ApiResponse<RequestDto>(requestDto, "تم الموافقة على طلب المساعدة بنجاح");
        }

        //* Reject Assistance Request
        public async Task<ApiResponse<RequestDto>> RejectAssistanceRequestAsync(int requestId, string rejectionReason)
        {
            if (string.IsNullOrEmpty(rejectionReason))
                return new ApiResponse<RequestDto>("سبب الرفض مطلوب", 200);


            var request = await _unitOfWork.GetRepository<Request, int>().GetByIdAsync(requestId);
            if (request == null)
                return new ApiResponse<RequestDto>("طلب المساعدة غير موجود", 200);


            if (request.Status != Status.Pending)
                return new ApiResponse<RequestDto>("لا يمكن رفض طلب غير معلق", 200);


            request.Status = Status.Rejected;
            request.RejectionReason = rejectionReason;
            request.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Request, int>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            var requestDto = request.Adapt<RequestDto>();
            await _notificationService.SendAssistanceRequestStatusUpdateAsync(request.RoomId, false, rejectionReason);
            return new ApiResponse<RequestDto>(requestDto, "تم رفض طلب المساعدة بنجاح");
        }

        //* Get Total Assistance Requests Count
        public async Task<ApiResponse<int>> GetTotalAssistanceRequestsCountAsync(int storeId)
        {
            if (storeId <= 0)
            {
                return new ApiResponse<int>("معرف المحل غير صالح", 200);
            }

            var count = (await _unitOfWork.GetRepository<Request, int>().FindAsync(ar => ar.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد طلبات المساعدة بنجاح");
        }

        #endregion
    }
}