using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IRoomService
    {
        Task<Room> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetAllRoomsAsync(int storeId, bool includeDeleted = false);
        Task AddRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(int id);
        Task RestoreRoomAsync(int id);
    }
}