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

        public async Task<IEnumerable<AssistanceRequest>> GetAssistanceRequestsByStoreAsync(int storeId, bool includeDeleted = false)
        {
            var rooms = await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.StoreId == storeId, includeDeleted);
            var roomIds = rooms.Select(r => r.Id).ToList();
            return await _unitOfWork.GetRepository<AssistanceRequest, int>().FindWithIncludesAsync(
                ar => roomIds.Contains(ar.RoomId),
                includeDeleted,
                ar => ar.Customer,
                ar => ar.Room
            );
        }

        public async Task AddAssistanceRequestAsync(AssistanceRequest assistanceRequest)
        {
            if (string.IsNullOrWhiteSpace(assistanceRequest.RequestType.Name))
                throw new CustomException("Request type is required.", 400);

            await _customerService.GetCustomerByIdAsync(assistanceRequest.CustomerId);
            await _roomService.GetRoomByIdAsync(assistanceRequest.RoomId);


            await _unitOfWork.GetRepository<AssistanceRequest, int>().AddAsync(assistanceRequest);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAssistanceRequestAsync(AssistanceRequest assistanceRequest)
        {
            var existingAssistanceRequest = await GetAssistanceRequestByIdAsync(assistanceRequest.Id);
            if (string.IsNullOrWhiteSpace(assistanceRequest.RequestType.Name))
                throw new CustomException("Request type is required.", 400);

            await _customerService.GetCustomerByIdAsync(assistanceRequest.CustomerId);
            await _roomService.GetRoomByIdAsync(assistanceRequest.RoomId);



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