using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(id);
            if (room == null)
                throw new Exception("Room not found.");
            return room;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync(int storeId, bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.StoreId == storeId, includeDeleted);
        }

        public async Task AddRoomAsync(Room room)
        {
            await _unitOfWork.GetRepository<Room, int>().AddAsync(room);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(Room room)
        {
            var existingRoom = await GetRoomByIdAsync(room.Id);
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
            await _unitOfWork.GetRepository<Room, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}