using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByRoomAsync(int roomId, bool includeDeleted = false);
        Task<IEnumerable<Order>> GetOrdersByStoreAsync(int storeId, bool includeDeleted = false);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task RestoreOrderAsync(int id);
    }
}