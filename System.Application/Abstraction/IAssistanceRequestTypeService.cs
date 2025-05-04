using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs.Assistances;

namespace System.Application.Abstraction
{
    public interface IAssistanceRequestTypeService
    {
        Task<ApiResponse<AssistanceRequestType>> CreateAssistanceRequestTypeAsync(string name, int storeId);
        Task<ApiResponse<AssistanceRequestType>> UpdateAssistanceRequestTypeAsync(int typeId, string name);
        Task<ApiResponse<bool>> DeleteAssistanceRequestTypeAsync(int typeId);
        Task<ApiResponse<bool>> RestoreAssistanceRequestTypeAsync(int typeId);
        Task<ApiResponse<List<AssistanceRequestType>>> GetAllAssistanceRequestTypesAsync(int storeId);
        Task<ApiResponse<int>> GetTotalAssistanceRequestTypesCountAsync(int storeId);
        Task<ApiResponse<DefaultAssistanceRequestType>> CreateDefaultAssistanceTypeAsync(string name);
        Task<ApiResponse<DefaultAssistanceRequestType>> UpdateDefaultAssistanceTypeAsync(int typeId, string name);
        Task<ApiResponse<bool>> DeleteDefaultAssistanceTypeAsync(int typeId);
        Task<ApiResponse<bool>> RestoreDefaultAssistanceTypeAsync(int typeId);
        Task<ApiResponse<List<AssistanceDto>>> GetDefaultAssistanceTypesAsync();
        Task<ApiResponse<List<AssistanceDto>>> GetDefaultDeletedAssistanceTypesAsync();
    }
}