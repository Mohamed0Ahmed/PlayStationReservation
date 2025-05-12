using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs.Orders;

namespace System.Application.Abstraction
{
    public interface IOrderService
    {
        Task<ApiResponse<Order>> CreateOrderAsync(string phoneNumber, int roomId, IEnumerable<ItemsDto> items);
        Task<ApiResponse<IEnumerable<OrderDto>>> GetPendingOrdersAsync(int storeId);
        Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersAsync(int storeId, bool includeDeleted = false);
        Task<ApiResponse<OrderDto>> ApproveOrderAsync(int orderId);
        Task<ApiResponse<OrderDto>> RejectOrderAsync(int orderId, string rejectionReason);
        Task<ApiResponse<int>> GetTotalOrdersCountAsync(int storeId);
    }
}