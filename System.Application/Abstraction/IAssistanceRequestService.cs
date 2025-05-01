using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IAssistanceRequestService
    {
        Task<ApiResponse<AssistanceRequest>> CreateAssistanceRequestAsync(int roomId, int requestTypeId);
        Task<ApiResponse<List<AssistanceRequest>>> GetPendingAssistanceRequestsAsync(int storeId);
        Task<ApiResponse<List<AssistanceRequest>>> GetAssistanceRequestsAsync(int storeId, bool includeDeleted = false);
        Task<ApiResponse<AssistanceRequest>> ApproveAssistanceRequestAsync(int requestId);
        Task<ApiResponse<AssistanceRequest>> RejectAssistanceRequestAsync(int requestId, string rejectionReason);
        Task<ApiResponse<int>> GetTotalAssistanceRequestsCountAsync(int storeId);
    }
}