using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IAssistanceRequestTypeService
    {
        Task<ApiResponse<AssistanceRequestType>> CreateAssistanceRequestTypeAsync(string name, int storeId);
        Task<ApiResponse<AssistanceRequestType>> UpdateAssistanceRequestTypeAsync(int typeId, string name);
        Task<ApiResponse<bool>> DeleteAssistanceRequestTypeAsync(int typeId);
        Task<ApiResponse<List<AssistanceRequestType>>> GetAllAssistanceRequestTypesAsync(int storeId);
        Task<ApiResponse<int>> GetTotalAssistanceRequestTypesCountAsync(int storeId);
    }
}