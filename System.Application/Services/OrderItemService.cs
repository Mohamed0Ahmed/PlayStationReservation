using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMenuItemService _menuItemService;

        public OrderItemService(IUnitOfWork unitOfWork, IMenuItemService menuItemService)
        {
            _unitOfWork = unitOfWork;
            _menuItemService = menuItemService;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderAsync(int orderId, bool includeDeleted = false)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdAsync(orderId);
            if (order == null)
                throw new CustomException("Order not found.", 404);

            return await _unitOfWork.GetRepository<OrderItem, int>().FindAsync(oi => oi.OrderId == orderId, includeDeleted);
        }

        public async Task AddOrderItemAsync(OrderItem orderItem)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdAsync(orderItem.OrderId);
            if (order == null)
                throw new CustomException("Order not found.", 404);

            await _menuItemService.GetMenuItemByIdAsync(orderItem.MenuItemId); 
            await _unitOfWork.GetRepository<OrderItem, int>().AddAsync(orderItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrderItemAsync(int orderId, int menuItemId)
        {
            var orderItem = (await _unitOfWork.GetRepository<OrderItem, int>().FindAsync(oi => oi.OrderId == orderId && oi.MenuItemId == menuItemId))
                .FirstOrDefault();
            if (orderItem == null)
                throw new CustomException("OrderItem not found.", 404);
            _unitOfWork.GetRepository<OrderItem, int>().Delete(orderItem);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}