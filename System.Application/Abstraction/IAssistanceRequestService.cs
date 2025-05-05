using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IRequestService
    {
        Task<ApiResponse<Request>> CreateAssistanceRequestAsync(int roomId, int requestTypeId);
        Task<ApiResponse<List<Request>>> GetPendingAssistanceRequestsAsync(int storeId);
        Task<ApiResponse<List<Request>>> GetAssistanceRequestsAsync(int storeId, bool includeDeleted = false);
        Task<ApiResponse<Request>> ApproveAssistanceRequestAsync(int requestId);
        Task<ApiResponse<Request>> RejectAssistanceRequestAsync(int requestId, string rejectionReason);
        Task<ApiResponse<int>> GetTotalAssistanceRequestsCountAsync(int storeId);
    }
}