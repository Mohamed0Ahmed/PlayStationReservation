using System.Infrastructure.Repositories;
using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderAsync(int orderId, bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<OrderItem, int>().FindAsync(oi => oi.OrderId == orderId, includeDeleted);
        }

        public async Task AddOrderItemAsync(OrderItem orderItem)
        {
            await _unitOfWork.GetRepository<OrderItem, int>().AddAsync(orderItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrderItemAsync(int orderId, int menuItemId)
        {
            var orderItem = (await _unitOfWork.GetRepository<OrderItem, int>().FindAsync(oi => oi.OrderId == orderId && oi.MenuItemId == menuItemId))
                .FirstOrDefault();
            if (orderItem == null)
                throw new Exception("OrderItem not found.");
            _unitOfWork.GetRepository<OrderItem, int>().Delete(orderItem);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}