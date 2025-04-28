using System.Infrastructure.Repositories;
using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class AssistanceRequestService : IAssistanceRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssistanceRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AssistanceRequest> GetAssistanceRequestByIdAsync(int id)
        {
            var assistanceRequest = await _unitOfWork.GetRepository<AssistanceRequest, int>().GetByIdAsync(id);
            if (assistanceRequest == null)
                throw new Exception("AssistanceRequest not found.");
            return assistanceRequest;
        }

        public async Task<IEnumerable<AssistanceRequest>> GetAssistanceRequestsByRoomAsync(int roomId, bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<AssistanceRequest, int>().FindAsync(ar => ar.RoomId == roomId, includeDeleted);
        }

        public async Task AddAssistanceRequestAsync(AssistanceRequest assistanceRequest)
        {
            await _unitOfWork.GetRepository<AssistanceRequest, int>().AddAsync(assistanceRequest);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAssistanceRequestAsync(AssistanceRequest assistanceRequest)
        {
            var existingAssistanceRequest = await GetAssistanceRequestByIdAsync(assistanceRequest.Id);
            existingAssistanceRequest.CustomerId = assistanceRequest.CustomerId;
            existingAssistanceRequest.RoomId = assistanceRequest.RoomId;
            existingAssistanceRequest.RequestType = assistanceRequest.RequestType;
            existingAssistanceRequest.Status = assistanceRequest.Status;
            existingAssistanceRequest.RejectionReason = assistanceRequest.RejectionReason;
            existingAssistanceRequest.RequestDate = assistanceRequest.RequestDate;
            _unitOfWork.GetRepository<AssistanceRequest, int>().Update(existingAssistanceRequest);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAssistanceRequestAsync(int id)
        {
            var assistanceRequest = await GetAssistanceRequestByIdAsync(id);
            _unitOfWork.GetRepository<AssistanceRequest, int>().Delete(assistanceRequest);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAssistanceRequestAsync(int id)
        {
            await _unitOfWork.GetRepository<AssistanceRequest, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}