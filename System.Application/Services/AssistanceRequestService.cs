using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Enums;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class AssistanceRequestService : IAssistanceRequestService
    {
        private readonly IRepository<AssistanceRequest, int> _assistanceRequestRepository;
        private readonly IRepository<AssistanceRequestType, int> _requestTypeRepository;
        private readonly INotificationService _notificationService;

        public AssistanceRequestService(
            IRepository<AssistanceRequest, int> assistanceRequestRepository,
            IRepository<AssistanceRequestType, int> requestTypeRepository,
            INotificationService notificationService)
        {
            _assistanceRequestRepository = assistanceRequestRepository;
            _requestTypeRepository = requestTypeRepository;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<AssistanceRequest>> CreateAssistanceRequestAsync(int customerId, int roomId, int requestTypeId)
        {
            var requestType = await _requestTypeRepository.GetByIdAsync(requestTypeId);
            if (requestType == null)
            {
                return new ApiResponse<AssistanceRequest>("نوع المساعدة غير موجود", 404);
            }

            var request = new AssistanceRequest
            {
                CustomerId = customerId,
                RoomId = roomId,
                RequestTypeId = requestTypeId,
                Status = AssistanceRequestStatus.Pending,
                RequestDate = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            await _assistanceRequestRepository.AddAsync(request);
            await _notificationService.SendAssistanceRequestNotificationAsync(requestType.StoreId, roomId);
            return new ApiResponse<AssistanceRequest>(request, "تم إضافة طلب المساعدة بنجاح", 201);
        }

        public async Task<ApiResponse<List<AssistanceRequest>>> GetPendingAssistanceRequestsAsync(int storeId)
        {
            var requests = await _assistanceRequestRepository.FindWithIncludesAsync(
                predicate: ar => ar.RequestType.StoreId == storeId && ar.Status == 0,
                include: q => q.Include(ar => ar.Customer).Include(ar => ar.Room).Include(ar => ar.RequestType),
                includeDeleted: false);

            return new ApiResponse<List<AssistanceRequest>>(requests.ToList());
        }

        public async Task<ApiResponse<List<AssistanceRequest>>> GetAssistanceRequestsAsync(int storeId, bool includeDeleted = false)
        {
            var requests = await _assistanceRequestRepository.FindWithIncludesAsync(
                predicate: ar => ar.RequestType.StoreId == storeId,
                include: q => q.Include(ar => ar.Customer).Include(ar => ar.Room).Include(ar => ar.RequestType),
                includeDeleted: includeDeleted);

            return new ApiResponse<List<AssistanceRequest>>(requests.ToList());
        }

        public async Task<ApiResponse<AssistanceRequest>> ApproveAssistanceRequestAsync(int requestId)
        {
            var request = await _assistanceRequestRepository.GetByIdWithIncludesAsync(requestId, include: q => q.Include(ar => ar.Room));
            if (request == null)
            {
                return new ApiResponse<AssistanceRequest>("طلب المساعدة غير موجود", 404);
            }

            request.Status = AssistanceRequestStatus.Accepted;
            request.LastModifiedOn = DateTime.UtcNow;
            _assistanceRequestRepository.Update(request);

            await _notificationService.SendAssistanceRequestStatusUpdateAsync(request.RoomId, true);
            return new ApiResponse<AssistanceRequest>(request, "تم الموافقة على طلب المساعدة بنجاح");
        }

        public async Task<ApiResponse<AssistanceRequest>> RejectAssistanceRequestAsync(int requestId, string rejectionReason)
        {
            var request = await _assistanceRequestRepository.GetByIdWithIncludesAsync(requestId, include: q => q.Include(ar => ar.Room));
            if (request == null)
            {
                return new ApiResponse<AssistanceRequest>("طلب المساعدة غير موجود", 404);
            }

            request.Status = AssistanceRequestStatus.Rejected;
            request.RejectionReason = rejectionReason;
            request.LastModifiedOn = DateTime.UtcNow;
            _assistanceRequestRepository.Update(request);

            await _notificationService.SendAssistanceRequestStatusUpdateAsync(request.RoomId, false, rejectionReason);
            return new ApiResponse<AssistanceRequest>(request, "تم رفض طلب المساعدة بنجاح");
        }

        public async Task<ApiResponse<int>> GetTotalAssistanceRequestsCountAsync(int storeId)
        {
            var count = (await _assistanceRequestRepository.FindAsync(ar => ar.RequestType.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد طلبات المساعدة بنجاح");
        }
    }
}