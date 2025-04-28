using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderAsync(int orderId, bool includeDeleted = false);
        Task AddOrderItemAsync(OrderItem orderItem);
        Task DeleteOrderItemAsync(int orderId, int menuItemId);
    }
}