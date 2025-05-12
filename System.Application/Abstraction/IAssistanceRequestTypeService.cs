using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs.Assistances;

namespace System.Application.Abstraction
{
    public interface IAssistanceRequestTypeService
    {
        Task<ApiResponse<AssistanceDto>> CreateAssistanceRequestTypeAsync(string name, int storeId);
        Task<ApiResponse<AssistanceDto>> UpdateAssistanceRequestTypeAsync(int typeId, string name);
        Task<ApiResponse<bool>> DeleteAssistanceRequestTypeAsync(int typeId);
        Task<ApiResponse<bool>> RestoreAssistanceRequestTypeAsync(int typeId);
        Task<ApiResponse<IEnumerable<AssistanceDto>>> GetAllAssistanceRequestTypesAsync(int storeId);
        Task<ApiResponse<IEnumerable<AssistanceDto>>> GetAllDeletedAssistanceRequestTypesAsync(int storeId);
        Task<ApiResponse<int>> GetTotalAssistanceRequestTypesCountAsync(int storeId);
        Task<ApiResponse<DefaultAssistanceRequestType>> CreateDefaultAssistanceTypeAsync(string name);
        Task<ApiResponse<DefaultAssistanceRequestType>> UpdateDefaultAssistanceTypeAsync(int typeId, string name);
        Task<ApiResponse<bool>> DeleteDefaultAssistanceTypeAsync(int typeId);
        Task<ApiResponse<bool>> RestoreDefaultAssistanceTypeAsync(int typeId);
        Task<ApiResponse<IEnumerable<AssistanceDto>>> GetDefaultAssistanceTypesAsync();
        Task<ApiResponse<IEnumerable<AssistanceDto>>> GetDefaultDeletedAssistanceTypesAsync();
    }
}