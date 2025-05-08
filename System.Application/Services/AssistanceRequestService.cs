using System.Application.Abstraction;
using System.Domain.Enums;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;

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
        public async Task<ApiResponse<Request>> CreateAssistanceRequestAsync(int roomId, int requestTypeId)
        {
            if (roomId <= 0 || requestTypeId <= 0)
                return new ApiResponse<Request>("مفيش غرفة او مساعدة بالاي دي ده", 400);


            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);

            if (room == null)
                return new ApiResponse<Request>("الغرفة غير موجودة", 404);


            // التحقق من نوع المساعدة (Custom أو Default)
            var customType = await _unitOfWork.GetRepository<AssistanceRequestType, int>().GetByIdAsync(requestTypeId);
            if (customType == null)
            {
                var defaultType = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetByIdAsync(requestTypeId);
                if (defaultType == null)
                    return new ApiResponse<Request>("نوع المساعدة غير موجود", 404);
            }


            var request = new Request
            {
                StoreId= room.StoreId,
                RoomId = roomId,
                RequestTypeId = requestTypeId,
                Status = Status.Pending,
                RequestDate = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Request, int>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendAssistanceRequestNotificationAsync(room.StoreId, roomId);
            return new ApiResponse<Request>(request, "تم إضافة طلب المساعدة بنجاح", 201);
        }

        //* Get Pending Assistance Requests
        public async Task<ApiResponse<IEnumerable<Request>>> GetPendingAssistanceRequestsAsync(int storeId)
        {
            var requests = await _unitOfWork.GetRepository<Request, int>().FindAsync(
                predicate: ar => ar.StoreId == storeId && ar.Status == Status.Pending);

            if (!requests.Any())
                return new ApiResponse<IEnumerable<Request>>("لا يوجد طلبات مساعدة حاليا", 404);


            return new ApiResponse<IEnumerable<Request>>(requests, "المساعدات المطلوبة حاليا");
        }

        //* Get All Assistance Requests
        public async Task<ApiResponse<IEnumerable<Request>>> GetAssistanceRequestsAsync(int storeId, bool includeDeleted = false)
        {
            if (storeId <= 0)
                return new ApiResponse<IEnumerable<Request>>("المحل ده مش موجود", 400);


            var requests = await _unitOfWork.GetRepository<Request, int>().FindWithIncludesAsync(
                predicate: ar => ar.StoreId == storeId);

            if (!requests.Any())
                return new ApiResponse<IEnumerable<Request>>("لا يوجد طلبات مساعدة", 404);


            return new ApiResponse<IEnumerable<Request>>(requests, "تم جلب طلبات المساعدة بنجاح");
        }

        //* Approve Assistance Request
        public async Task<ApiResponse<Request>> ApproveAssistanceRequestAsync(int requestId)
        {
            var request = await _unitOfWork.GetRepository<Request, int>().GetByIdWithIncludesAsync(requestId);

            if (request == null)
                return new ApiResponse<Request>("طلب المساعدة غير موجود", 404);


            if (request.Status != Status.Pending)
                return new ApiResponse<Request>("لا يمكن الموافقة على طلب غير معلق", 400);

            request.Status = Status.Accepted;
            request.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Request, int>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendAssistanceRequestStatusUpdateAsync(request.RoomId, true);
            return new ApiResponse<Request>(request, "تم الموافقة على طلب المساعدة بنجاح");
        }

        //* Reject Assistance Request
        public async Task<ApiResponse<Request>> RejectAssistanceRequestAsync(int requestId, string rejectionReason)
        {
            if (requestId <= 0)
                return new ApiResponse<Request>("ادخل البيانات بشكل صحيح", 400);


            if (string.IsNullOrEmpty(rejectionReason))
                return new ApiResponse<Request>("سبب الرفض مطلوب", 400);


            var request = await _unitOfWork.GetRepository<Request, int>().GetByIdAsync(requestId);
            if (request == null)
                return new ApiResponse<Request>("طلب المساعدة غير موجود", 404);


            if (request.Status != Status.Pending)
                return new ApiResponse<Request>("لا يمكن رفض طلب غير معلق", 400);


            request.Status = Status.Rejected;
            request.RejectionReason = rejectionReason;
            request.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Request, int>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendAssistanceRequestStatusUpdateAsync(request.RoomId, false, rejectionReason);
            return new ApiResponse<Request>(request, "تم رفض طلب المساعدة بنجاح");
        }

        //* Get Total Assistance Requests Count
        public async Task<ApiResponse<int>> GetTotalAssistanceRequestsCountAsync(int storeId)
        {
            if (storeId <= 0)
            {
                return new ApiResponse<int>("معرف المحل غير صالح", 400);
            }

            var count = (await _unitOfWork.GetRepository<Request, int>().FindAsync(ar => ar.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد طلبات المساعدة بنجاح");
        }

        #endregion
    }
}