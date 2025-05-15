using System.Shared;
using System.Shared.DTOs.Requests;

namespace System.Application.Abstraction
{
    public interface IRequestService
    {
        Task<ApiResponse<RequestDto>> CreateAssistanceRequestAsync(int roomId, int requestTypeId);
        Task<ApiResponse<IEnumerable<RequestDto>>> GetPendingAssistanceRequestsAsync(int storeId);
        Task<ApiResponse<IEnumerable<RequestDto>>> GetAssistanceRequestsAsync(int storeId, bool includeDeleted = false);
        Task<ApiResponse<RequestDto>> ApproveAssistanceRequestAsync(int requestId);
        Task<ApiResponse<RequestDto>> RejectAssistanceRequestAsync(int requestId, string rejectionReason);
        Task<ApiResponse<int>> GetTotalAssistanceRequestsCountAsync(int storeId);
    }
}