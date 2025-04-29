using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreService _storeService;

        public RoomService(IUnitOfWork unitOfWork, IStoreService storeService)
        {
            _unitOfWork = unitOfWork;
            _storeService = storeService;
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdWithIncludesAsync(id, false, r => r.Orders, r => r.AssistanceRequests);
            if (room == null)
                throw new CustomException("Room not found.", 404);
            return room;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync(int storeId, bool includeDeleted = false)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId, includeDeleted);
            return await _unitOfWork.GetRepository<Room, int>().FindWithIncludesAsync(r => r.StoreId == storeId, includeDeleted, r => r.Orders, r => r.AssistanceRequests);
        }

        public async Task AddRoomAsync(Room room)
        {
            if (string.IsNullOrWhiteSpace(room.Username))
                throw new CustomException("Room username is required.", 400);
            if (string.IsNullOrWhiteSpace(room.Password))
                throw new CustomException("Room password is required.", 400);

            // Check for duplicate Username and StoreId
            var existingRoom = (await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.Username == room.Username && r.StoreId == room.StoreId && !r.IsDeleted)).FirstOrDefault();
            if (existingRoom != null)
                throw new CustomException($"A room with the username '{room.Username}' already exists for this store.", 400);

            await _storeService.GetStoreByIdAsync(room.StoreId);
            await _unitOfWork.GetRepository<Room, int>().AddAsync(room);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(Room room)
        {
            var existingRoom = await GetRoomByIdAsync(room.Id);
            if (string.IsNullOrWhiteSpace(room.Username))
                throw new CustomException("Room username is required.", 400);
            if (string.IsNullOrWhiteSpace(room.Password))
                throw new CustomException("Room password is required.", 400);

            // Check for duplicate Username and StoreId (excluding the current room)
            var duplicateRoom = (await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.Username == room.Username && r.StoreId == room.StoreId && r.Id != room.Id && !r.IsDeleted)).FirstOrDefault();
            if (duplicateRoom != null)
                throw new CustomException($"Another room with the username '{room.Username}' already exists for this store.", 400);

            await _storeService.GetStoreByIdAsync(room.StoreId);
            existingRoom.Username = room.Username;
            existingRoom.Password = room.Password;
            existingRoom.StoreId = room.StoreId;
            _unitOfWork.GetRepository<Room, int>().Update(existingRoom);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await GetRoomByIdAsync(id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Soft delete all related Orders
                var orders = await _unitOfWork.GetRepository<Order, int>().FindAsync(o => o.RoomId == id && !o.IsDeleted);
                foreach (var order in orders)
                {
                    _unitOfWork.GetRepository<Order, int>().Delete(order);
                }

                // Soft delete all related AssistanceRequests
                var assistanceRequests = await _unitOfWork.GetRepository<AssistanceRequest, int>().FindAsync(ar => ar.RoomId == id && !ar.IsDeleted);
                foreach (var assistanceRequest in assistanceRequests)
                {
                    _unitOfWork.GetRepository<AssistanceRequest, int>().Delete(assistanceRequest);
                }

                _unitOfWork.GetRepository<Room, int>().Delete(room);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException("Failed to delete room.", 500);
            }
        }

        public async Task RestoreRoomAsync(int id)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(id, true);
            if (room == null)
                throw new CustomException("Room not found.", 404);
            if (!room.IsDeleted)
                throw new CustomException("Room is not deleted.", 400);

            await _unitOfWork.GetRepository<Room, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}