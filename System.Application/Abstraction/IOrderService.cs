using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs.Orders;

namespace System.Application.Abstraction
{
    public interface IOrderService
    {
        Task<ApiResponse<Order>> CreateOrderAsync(string phoneNumber, int roomId, List<ItemsDto> items);
        Task<ApiResponse<List<Order>>> GetPendingOrdersAsync(int storeId);
        Task<ApiResponse<List<Order>>> GetOrdersAsync(int storeId, bool includeDeleted = false);
        Task<ApiResponse<Order>> ApproveOrderAsync(int orderId);
        Task<ApiResponse<Order>> RejectOrderAsync(int orderId, string rejectionReason);
        Task<ApiResponse<int>> GetTotalOrdersCountAsync(int storeId);
    }
}