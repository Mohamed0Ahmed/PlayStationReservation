using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IOrderService
    {
        Task<ApiResponse<Order>> CreateOrderAsync(string phoneNumber, int roomId, List<(int menuItemId, int quantity)> items);
        Task<ApiResponse<List<Order>>> GetPendingOrdersAsync(int storeId);
        Task<ApiResponse<List<Order>>> GetOrdersAsync(int storeId, bool includeDeleted = false);
        Task<ApiResponse<Order>> ApproveOrderAsync(int orderId, decimal totalAmount);
        Task<ApiResponse<Order>> RejectOrderAsync(int orderId, string rejectionReason);
        Task<ApiResponse<int>> GetTotalOrdersCountAsync(int storeId);
    }
}