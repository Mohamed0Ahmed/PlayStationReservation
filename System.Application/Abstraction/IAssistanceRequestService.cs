using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IAssistanceRequestService
    {
        Task<AssistanceRequest> GetAssistanceRequestByIdAsync(int id);
        Task<IEnumerable<AssistanceRequest>> GetAssistanceRequestsByRoomAsync(int roomId, bool includeDeleted = false);
        Task<IEnumerable<AssistanceRequest>> GetAssistanceRequestsByStoreAsync(int storeId, bool includeDeleted = false);
        Task AddAssistanceRequestAsync(AssistanceRequest assistanceRequest);
        Task UpdateAssistanceRequestAsync(AssistanceRequest assistanceRequest);
        Task DeleteAssistanceRequestAsync(int id);
        Task RestoreAssistanceRequestAsync(int id);
    }
}