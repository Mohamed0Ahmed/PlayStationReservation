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
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(id);
            if (room == null)
                throw new CustomException("Room not found.", 404);
            return room;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync(int storeId, bool includeDeleted = false)
        {
            await _storeService.GetStoreByIdAsync(storeId  , includeDeleted);
            return await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.StoreId == storeId, includeDeleted);
        }

        public async Task AddRoomAsync(Room room)
        {
            if (string.IsNullOrWhiteSpace(room.Username))
                throw new CustomException("Room username is required.", 400);
            if (string.IsNullOrWhiteSpace(room.Password))
                throw new CustomException("Room password is required.", 400);

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
            _unitOfWork.GetRepository<Room, int>().Delete(room);
            await _unitOfWork.SaveChangesAsync();
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