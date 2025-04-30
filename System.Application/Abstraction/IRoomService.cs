using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IRoomService
    {
        Task<Room> GetRoomByIdAsync(int id, bool includeDeleted = false);
        Task<Room> GetRoomByNameAsync(string userName, string password, int id, bool includeDeleted = false);
        Task<IEnumerable<Room>> GetAllRoomsAsync(int storeId, bool includeDeleted = false);
        Task AddRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(int id);
        Task RestoreRoomAsync(int id);
    }
}