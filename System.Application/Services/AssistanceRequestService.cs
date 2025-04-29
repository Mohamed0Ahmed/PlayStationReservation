using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class AssistanceRequestService : IAssistanceRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;
        private readonly IRoomService _roomService;

        public AssistanceRequestService(IUnitOfWork unitOfWork, ICustomerService customerService, IRoomService roomService)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _roomService = roomService;
        }

        public async Task<AssistanceRequest> GetAssistanceRequestByIdAsync(int id)
        {
            var assistanceRequest = await _unitOfWork.GetRepository<AssistanceRequest, int>().GetByIdAsync(id);
            if (assistanceRequest == null)
                throw new CustomException("AssistanceRequest not found.", 404);
            return assistanceRequest;
        }

        public async Task<IEnumerable<AssistanceRequest>> GetAssistanceRequestsByRoomAsync(int roomId, bool includeDeleted = false)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            return await _unitOfWork.GetRepository<AssistanceRequest, int>().FindAsync(ar => ar.RoomId == roomId, includeDeleted);
        }

        public async Task AddAssistanceRequestAsync(AssistanceRequest assistanceRequest)
        {
            if (string.IsNullOrWhiteSpace(assistanceRequest.RequestType))
                throw new CustomException("Request type is required.", 400);

            await _customerService.GetCustomerByIdAsync(assistanceRequest.CustomerId);
            await _roomService.GetRoomByIdAsync(assistanceRequest.RoomId);

            // Check for duplicate request (same RequestType, RoomId, and same day)
            var today = DateTime.UtcNow.Date;
            var existingRequest = (await _unitOfWork.GetRepository<AssistanceRequest, int>().FindAsync(ar =>
                ar.RequestType == assistanceRequest.RequestType &&
                ar.RoomId == assistanceRequest.RoomId &&
                ar.RequestDate.Date == today &&
                !ar.IsDeleted)).FirstOrDefault();
            if (existingRequest != null)
                throw new CustomException($"An assistance request of type '{assistanceRequest.RequestType}' already exists for this room today.", 400);

            await _unitOfWork.GetRepository<AssistanceRequest, int>().AddAsync(assistanceRequest);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAssistanceRequestAsync(AssistanceRequest assistanceRequest)
        {
            var existingAssistanceRequest = await GetAssistanceRequestByIdAsync(assistanceRequest.Id);
            if (string.IsNullOrWhiteSpace(assistanceRequest.RequestType))
                throw new CustomException("Request type is required.", 400);

            await _customerService.GetCustomerByIdAsync(assistanceRequest.CustomerId);
            await _roomService.GetRoomByIdAsync(assistanceRequest.RoomId);

            // Check for duplicate request (excluding the current request)
            var today = DateTime.UtcNow.Date;
            var duplicateRequest = (await _unitOfWork.GetRepository<AssistanceRequest, int>().FindAsync(ar =>
                ar.RequestType == assistanceRequest.RequestType &&
                ar.RoomId == assistanceRequest.RoomId &&
                ar.RequestDate.Date == today &&
                ar.Id != assistanceRequest.Id &&
                !ar.IsDeleted)).FirstOrDefault();
            if (duplicateRequest != null)
                throw new CustomException($"Another assistance request of type '{assistanceRequest.RequestType}' already exists for this room today.", 400);

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
            var assistanceRequest = await _unitOfWork.GetRepository<AssistanceRequest, int>().GetByIdAsync(id, true);
            if (assistanceRequest == null)
                throw new CustomException("AssistanceRequest not found.", 404);
            if (!assistanceRequest.IsDeleted)
                throw new CustomException("AssistanceRequest is not deleted.", 400);

            await _unitOfWork.GetRepository<AssistanceRequest, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}